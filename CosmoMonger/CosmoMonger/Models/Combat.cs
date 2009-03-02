//-----------------------------------------------------------------------
// <copyright file="Combat.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extension of the partial LINQ class Combat.
    /// This class handles the combat conditions and scenerios.
    /// </summary>
    public partial class Combat
    {
        /// <summary>
        /// The default number of points given per turn
        /// </summary>
        public const int PointsPerTurn = 20;

        /// <summary>
        /// This enum describes the meaning of the Combat.Status field
        /// </summary>
        public enum CombatStatus
        {
            /// <summary>
            /// Combat is still in progress
            /// </summary>
            Incomplete = 0,

            /// <summary>
            /// Combat is over. Current Turn Ship has destroyed the other ship.
            /// </summary>
            ShipDestroyed = 1,

            /// <summary>
            /// Combat is over. Current Turn Ship chose to pickup cargo jettisoned by the other ship. 
            /// This has allowed the other ship to escape.
            /// </summary>
            CargoPickup = 2,

            /// <summary>
            /// Combat is over. Current Turn Ship has escaped combat by fully charging JumpDrive.
            /// </summary>
            ShipFled = 3,

            /// <summary>
            /// Combat is over. Current Turn Ship has accepted the other ships surrender.
            /// </summary>
            ShipSurrendered = 4
        }

        /// <summary>
        /// Gets a list of Goods that can be picked up by the opposing player.
        /// If the goods are not picked up, the goods are deleted from the system.
        /// </summary>
        /// <value>The cargo to pickup. Null if there is no cargo jettisoned</value>
        public ShipGood[] CargoToPickup
        {
            get
            {
                if (this.JettisonCargo)
                {
                    return this.ShipOther.GetGoods();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a reference to the current Ship ('Player') turn.
        /// We reference Ships rather than Players because NPCs are not considered Players,
        /// but do own Ships and can fight.
        /// </summary>
        /// <value>The ship whose turn is currently is.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the Turn field is an invalid value</exception>
        public Ship ShipTurn
        {
            get
            {
                if (this.Turn == 0)
                {
                    return this.AttackerShip;
                }
                else if (this.Turn == 1)
                {
                    return this.DefenderShip;
                }

                throw new ArgumentOutOfRangeException("Turn");
            }
        }

        /// <summary>
        /// Gets a reference to the other Ship.
        /// We reference Ships rather than Players because NPCs are not considered Players,
        /// but do own Ships and can fight.
        /// </summary>
        /// <value>The ship whose turn it is not.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the Turn field is an invalid value</exception>
        public Ship ShipOther
        {
            get
            {
                if (this.Turn == 0)
                {
                    return this.DefenderShip;
                }
                else if (this.Turn == 1)
                {
                    return this.AttackerShip;
                }

                throw new ArgumentOutOfRangeException("Turn");
            }
        }

        /// <summary>
        /// Fires the primary weapon at the opposing ship.
        /// If the opposing ship is destoryed then the non-destoryed cargo and credits are
        /// picked up and victory is declared. If the opposing ship is player driven then
        /// the player is cloned on a nearby planet with a bank and given a small ship to get started again.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if combat is over</exception>
        /// /// <exception cref="ArgumentOutOfRangeException">Thrown if not enough turn points are left to fire weapon</exception>
        public void FireWeapon()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Weapon firingWeapon = this.ShipTurn.Weapon;

            // Check there are enough turn points to fire weapon
            if (this.TurnPointsLeft < firingWeapon.TurnCost)
            {
                throw new ArgumentOutOfRangeException("Not enough turn points left to fire weapon", "TurnCost");
            }

            // Activate other ship shields
            // Formula is: ShieldStrength * (1 - (ShieldDamage / 100))
            // which means 0% damage gives full power, 50% damage gives half power, 100% damage gives no shield power
            double shieldStrength = this.ShipOther.Shield.Strength * (1.0 - (this.ShipOther.DamageShield / 100.0));

            // Weapon: 8
            // Shield: 10

            // Power up weapon
            // Damage should be at least 1, even with powerful shields
            double weaponDamage = Math.Max(this.ShipTurn.Weapon.Power - shieldStrength, 1.0);

            // Apply damage
            // Half goes to shields and the rest goes to the hull
            // Max damage is 100%
            // TODO: Divide by .5?
            double newDamageShield = Math.Ceiling(this.ShipOther.DamageShield + (weaponDamage / 0.5));
            this.ShipOther.DamageShield = (int)Math.Min(newDamageShield, 100);
            
            double newDamageHull = this.ShipOther.DamageHull + ((weaponDamage / 1.5) * (this.ShipOther.DamageShield / 100.0));
            newDamageHull = Math.Ceiling(newDamageHull);
            this.ShipOther.DamageHull = (int)Math.Min(newDamageHull, 100);

            // Deduct turn points
            this.TurnPointsLeft -= firingWeapon.TurnCost;

            Logger.Write("Attacking ship fired weapon", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.FireWeapon",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId },
                    { "OtherShipId", this.ShipOther.ShipId },
                    { "ShieldStrength", shieldStrength },
                    { "WeaponDamage", weaponDamage },
                    { "NewDamageShield", newDamageShield },
                    { "NewDamageHull", newDamageHull },
                    { "TurnPointsLeft", this.TurnPointsLeft }
                }
            );

            try
            {
                // Save database changes
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to the combat record, this is invalid
                // and so we use our values and ignore the new data
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    // Refresh current values from database
                    occ.Resolve(RefreshMode.KeepCurrentValues);
                }
            }

            // Did we destory the other ship?
            if (this.ShipOther.Destroyed)
            {
                // Victory
                this.OtherShipDestroyed();
            } 
            else if (this.TurnPointsLeft <= 0)
            {
                // No more turn points left, end turn
                this.EndTurn();
            }
        }

        /// <summary>
        /// Gives up the rest of the turn and signals that the current ship is surrendering to the opposing ship.
        /// </summary>
        public void OfferSurrender()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Surrender flag must be not set
            if (this.Surrender)
            {
                throw new InvalidOperationException("Other ship already offered surrender");
            }

            this.Surrender = true;

            Logger.Write("Offered surrender", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.OfferSurrender",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId},
                    { "OtherShipId", this.ShipOther.ShipId }
                }
            );

            db.SubmitChanges();

            // Turn is ended
            this.SwapTurn();
        }

        /// <summary>
        /// Accept the surrender of the opposing ship. 
        /// This gives all the goods and credits aboard the opposing ship to the current ship and ends combat.
        /// </summary>
        public void AcceptSurrender()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Surrender flag must be set
            if (!this.Surrender)
            {
                throw new InvalidOperationException("No surrender offered");
            }

            Logger.Write("Accepted surrender", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.AcceptSurrender",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId},
                    { "OtherShipId", this.ShipOther.ShipId }
                }
            );

            // Move enemy cargo into our cargo bays
            foreach (ShipGood cargo in this.ShipOther.ShipGoods)
            {
                this.ShipTurn.AddGood(cargo.GoodId, cargo.Quantity);
            }

            // Delete the cargo from the other ship
            db.ShipGoods.DeleteAllOnSubmit(this.ShipOther.ShipGoods);

            // Combat has ended
            this.Status = CombatStatus.ShipSurrendered;

            db.SubmitChanges();
        }

        /// <summary>
        /// Jettison all of the ships current cargo. 
        /// This will allow the ship to escape if the opposing ship picks up the cargo.
        /// </summary>
        public void JettisonShipCargo()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Jettison Cargo flag must be not set
            if (this.JettisonCargo)
            {
                throw new InvalidOperationException("There is already cargo jettisoned");
            }

            // Check that the ship has cargo to jettison
            if (this.ShipTurn.ShipGoods.Sum(g => g.Quantity) == 0)
            {
                throw new InvalidOperationException("No ship cargo to jettison");
            }

            this.JettisonCargo = true;

            Logger.Write("Jettisoned cargo", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.JettisonShipCargo",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId},
                    { "OtherShipId", this.ShipOther.ShipId }
                }
            );

            db.SubmitChanges();

            // Turn is ended
            this.SwapTurn();
        }

        /// <summary>
        /// Pickup cargo jettisoned by opposing ship, this will end combat as the other ship will escape. 
        /// If the cargo is not picked up, it is deleted on the next turn.
        /// </summary>
        public void PickupCargo()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Jettison Cargo flag must be set
            if (!this.JettisonCargo)
            {
                throw new InvalidOperationException("No cargo jettisoned");
            }

            // Find cargo to pickup
            ShipGood[] pickCargo = this.CargoToPickup;
            if (pickCargo == null)
            {
                throw new InvalidOperationException("No cargo jettisoned");
            }

            // Move cargo into our cargo bays
            foreach (ShipGood cargo in pickCargo) 
            {
                this.ShipTurn.AddGood(cargo.GoodId, cargo.Quantity);
            }

            // Delete the space cargo from the database
            db.ShipGoods.DeleteAllOnSubmit(pickCargo);

            Logger.Write("Picked up cargo", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.PickupCargo",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId },
                    { "OtherShipId", this.ShipOther.ShipId },
                    { "TotalCargoCount", pickCargo.Sum(g => g.Quantity) }
                }
            );

            // Combat has ended
            this.Status = CombatStatus.CargoPickup;

            db.SubmitChanges();
        }

        /// <summary>
        /// Uses the rest of the current turn points to charge the jump drive. 
        /// If the jump drive becomes completely charged, the ship escapes and combat is ended.
        /// </summary>
        public void ChargeJumpdrive()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Get the current jumpdrive charge if any
            int currentCharge = this.ShipTurn.CurrentJumpDriveCharge ?? 0;

            // Alloc turn points to the charge of the JumpDrive
            this.ShipTurn.CurrentJumpDriveCharge = currentCharge + this.TurnPointsLeft;
            this.TurnPointsLeft = 0;

            // Did the jumpdrive fully charge?
            if (this.ShipTurn.CurrentJumpDriveCharge >= 100)
            {
                // TODO: This ship escapes combat
                this.Status = CombatStatus.ShipFled;
                throw new NotImplementedException();
            }

            db.SubmitChanges();

            this.SwapTurn();
        }

        /// <summary>
        /// Ends the current ships turn. Giving control to the other ship.
        /// This does check the surrender and cargo flags.
        /// </summary>
        public void EndTurn()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.SwapTurn();

            // Check if surrender was not accepted
            if (this.Surrender)
            {
                // Reset flag
                this.Surrender = false;
            }

            // Check if cargo was not picked up
            if (this.JettisonCargo)
            {
                // Delete ignored space goods
                db.ShipGoods.DeleteAllOnSubmit(this.CargoToPickup);

                // Reset flag
                this.JettisonCargo = false;
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Swaps the turn to the other ship, giving up any turn points left.
        /// This does not check the surrender or cargo flags.
        /// </summary>
        private void SwapTurn()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Swap the turn
            if (this.Turn == 0)
            {
                this.Turn = 1;
            }
            else if (this.Turn == 1)
            {
                this.Turn = 0;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Turn");
            }

            // Rest the turn point counter
            this.TurnPointsLeft = Combat.PointsPerTurn;

            db.SubmitChanges();
        }

        /// <summary>
        /// Called when the other ship has been destroyed. Current turn ship has won
        /// </summary>
        private void OtherShipDestroyed()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // No longer traveling, combat has ended
            this.ShipOther.TargetSystemId = null;
            this.ShipOther.TargetSystemArrivalTime = null;
            this.ShipTurn.TargetSystemId = null;
            this.ShipTurn.TargetSystemArrivalTime = null;

            // Move cargo into our cargo bays
            int cargoWorth = this.ShipOther.ShipGoods.Sum(g => (g.Quantity * g.Good.BasePrice));
            foreach (ShipGood cargo in this.ShipOther.ShipGoods)
            {
                this.ShipTurn.AddGood(cargo.GoodId, cargo.Quantity);
            }

            // Delete the extra cargo from the database
            db.ShipGoods.DeleteAllOnSubmit(this.ShipOther.ShipGoods);

            Player turnPlayer = this.ShipTurn.Players.SingleOrDefault();
            Player otherPlayer = this.ShipOther.Players.SingleOrDefault();
            if (turnPlayer != null && otherPlayer != null)
            {
                Logger.Write("Transfering losing player credits to winner", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.OtherShipDestroyed",
                    new Dictionary<string, object>
                    {
                        { "CombatId", this.CombatId },
                        { "TurnPlayerId", turnPlayer.PlayerId },
                        { "OtherPlayerId", otherPlayer.PlayerId },
                        { "OtherPlayerCashCredits", otherPlayer.CashCredits },
                        { "TurnPlayerCashCredits", turnPlayer.CashCredits }
                    }
                );

                // Take the other players credits
                turnPlayer.CashCredits += otherPlayer.CashCredits;
                otherPlayer.CashCredits = 0;

                try
                {
                    db.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException ex)
                {
                    ExceptionPolicy.HandleException(ex, "SQL Policy");

                    // Another thread has made changes to one of the player records
                    // Overwrite those changes
                    foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                    {
                        occ.Resolve(RefreshMode.KeepChanges);
                    }
                }

                // Relocate other player to nearest system with a bank
                CosmoSystem bankSystem = this.ShipOther.GetNearestBankSystem();

                // Save a reference to the players old ship
                Ship otherPlayerOldShip = otherPlayer.Ship;

                otherPlayer.Ship = null;

                Logger.Write("Relocating losing player to nearest system with bank", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.OtherShipDestroyed",
                    new Dictionary<string, object>
                    {
                        { "CombatId", this.CombatId },
                        { "TurnPlayerId", turnPlayer.PlayerId },
                        { "OtherPlayerId", otherPlayer.PlayerId },
                        { "BankSystemId", bankSystem.SystemId }
                    }
                );

                // Give the player a new ship in the bank system
                otherPlayer.CreateStartingShip(bankSystem);

                // Give the player some starting credits
                double cloneCredits = 2000 - ((otherPlayer.BankCredits + 1) / 5000.0 * 2000);

                // Ignore negative values
                cloneCredits = Math.Max(cloneCredits, 0);
                otherPlayer.CashCredits = (int)cloneCredits;

                Logger.Write("Giving losing player starting cash credits", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.OtherShipDestroyed",
                    new Dictionary<string, object>
                    {
                        { "CombatId", this.CombatId },
                        { "TurnPlayerId", turnPlayer.PlayerId },
                        { "OtherPlayerId", otherPlayer.PlayerId },
                        { "CashCredits", otherPlayer.CashCredits }
                    }
                );

                // Update player stats
                otherPlayer.ShipsLost++;
                otherPlayer.CargoLostWorth += cargoWorth;

                turnPlayer.ShipsDestroyed++;
                turnPlayer.CargoLootedWorth += cargoWorth;

                // Combat has ended
                this.Status = CombatStatus.ShipDestroyed;

                try
                {
                    db.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException ex)
                {
                    ExceptionPolicy.HandleException(ex, "SQL Policy");

                    // Another thread has made changes to the player or combat records
                    // Overwrite those changes
                    foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                    {
                        occ.Resolve(RefreshMode.KeepChanges);
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Non-player ship or NPC combat not supported");
            }
        }
    }
}
