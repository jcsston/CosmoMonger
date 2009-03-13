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
        /// The default amount of seconds given per turn
        /// </summary>
        public const int SecondsPerTurn = 30;

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
        /// Gets the turn time left.
        /// </summary>
        /// <value>The turn time left.</value>
        public TimeSpan TurnTimeLeft
        {
            get
            {
                return this.LastActionTime.AddSeconds(Combat.SecondsPerTurn) - DateTime.Now;
            }
        }

        /// <summary>
        /// Checks the turn time left.
        /// </summary>
        /// <returns></returns>
        public void CheckTurnTimeLeft()
        {
            // Check if any time is left
            if (this.TurnTimeLeft.TotalSeconds < 0 && this.Status == CombatStatus.Incomplete)
            {
                // No time left in turn, auto-charge JumpDrive
                this.ChargeJumpDrive();
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

            // Update turn action time
            this.LastActionTime = DateTime.Now;

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
        /// <exception cref="InvalidOperationException">Thrown when combat is over or other ship has surrendered</exception>
        public void OfferSurrender()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Surrender flag must be not set
            if (this.Surrendered)
            {
                throw new InvalidOperationException("Other ship already offered surrender");
            }

            this.Surrendered = true;

            Logger.Write("Offered surrender", "Model", 150, 0, TraceEventType.Verbose, "InProgressCombat.OfferSurrender",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId},
                    { "OtherShipId", this.ShipOther.ShipId }
                }
            );

            // Update turn action time
            this.LastActionTime = DateTime.Now;

            db.SubmitChanges();

            // Turn is ended
            this.SwapTurn();
        }

        /// <summary>
        /// Accept the surrender of the opposing ship. 
        /// This gives all the goods and credits aboard the opposing ship to the current ship and ends combat.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when combat is over or no surrender has been offered</exception>
        public void AcceptSurrender()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Surrender flag must be set
            if (!this.Surrendered)
            {
                throw new InvalidOperationException("No surrender offered");
            }

            Logger.Write("Accepted surrender", "Model", 150, 0, TraceEventType.Verbose, "Combat.AcceptSurrender",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId},
                    { "OtherShipId", this.ShipOther.ShipId }
                }
            );

            // In space the cargo will go
            this.SendCargoIntoSpace(this.ShipOther);

            // Move enemy cargo from space into our cargo bays
            this.LoadCargo();

            // Combat has ended
            this.Status = CombatStatus.ShipSurrendered;

            // Update turn action time
            this.LastActionTime = DateTime.Now;

            db.SubmitChanges();
        }

        /// <summary>
        /// Jettison all of the ships current cargo. 
        /// This will allow the ship to escape if the opposing ship picks up the cargo.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when combat is over or there is no cargo to jettison or cargo has already need jettisoned</exception>
        public void JettisonCargo()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Jettison Cargo flag must be not set
            if (this.CargoJettisoned)
            {
                throw new InvalidOperationException("There is already cargo jettisoned");
            }

            // Check that the ship has cargo to jettison
            if (this.ShipTurn.ShipGoods.Sum(g => g.Quantity) == 0)
            {
                throw new InvalidOperationException("No ship cargo to jettison");
            }

            // Sending cargo into space
            this.SendCargoIntoSpace(this.ShipTurn);

            this.CargoJettisoned = true;

            Logger.Write("Jettisoned cargo", "Model", 150, 0, TraceEventType.Verbose, "Combat.JettisonShipCargo",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId},
                    { "OtherShipId", this.ShipOther.ShipId }
                }
            );

            // Update turn action time
            this.LastActionTime = DateTime.Now;

            db.SubmitChanges();

            // Turn is ended
            this.SwapTurn();
        }

        /// <summary>
        /// Pickup cargo jettisoned by opposing ship, this will end combat as the other ship will escape. 
        /// If the cargo is not picked up, it is deleted on the next turn.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when combat is over or there is no cargo to jettison</exception>
        public void PickupCargo()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Jettison Cargo flag must be set
            if (!this.CargoJettisoned)
            {
                throw new InvalidOperationException("No cargo jettisoned");
            }

            // Find cargo to pickup
            this.LoadCargo();

            // Combat has ended
            this.Status = CombatStatus.CargoPickup;

            // Update turn action time
            this.LastActionTime = DateTime.Now;

            db.SubmitChanges();

            // Ensure both ships are no longer traveling
            this.ShipTurn.CheckIfTraveling();
            this.ShipOther.CheckIfTraveling();
        }

        /// <summary>
        /// Uses the rest of the current turn points to charge the jump drive. 
        /// If the jump drive becomes completely charged, the ship escapes and combat is ended.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when combat is over</exception>
        public void ChargeJumpDrive()
        {
            // Check that the combat is still in-progress
            if (this.Status != CombatStatus.Incomplete)
            {
                throw new InvalidOperationException("Combat is over");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Alloc turn points to the charge of the JumpDrive
            this.ShipTurn.CurrentJumpDriveCharge += this.TurnPointsLeft;
            this.TurnPointsLeft = 0;

            // Did the jumpdrive fully charge?
            if (this.ShipTurn.CurrentJumpDriveCharge >= 100)
            {
                // This ship escapes combat
                this.Status = CombatStatus.ShipFled;

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

                // Repair both ships
                this.ShipTurn.Repair();
                this.ShipOther.Repair();

                // Reset jump drive charges
                this.ShipOther.CurrentJumpDriveCharge = 0;
                this.ShipTurn.CurrentJumpDriveCharge = 0;

                // Check how much the longer the ship needed prep for jumping
                if (this.ShipTurn.TargetSystemArrivalTime.HasValue && this.ShipTurn.TargetSystemArrivalTime > DateTime.Now)
                {
                    // The ship still needs time to prep,
                    // Combat is non real-time so we will cheat here and make the ship instantly jump
                    this.ShipTurn.TargetSystemArrivalTime = DateTime.Now.AddSeconds(-1);
                }

                // Ensure that the ship traveling is in valid state
                this.ShipTurn.CheckIfTraveling();
            }
            else
            {
                // Ship did not escape yet, so it's the other ships turn
                this.EndTurn();
            }

            // Update turn action time
            this.LastActionTime = DateTime.Now;

            db.SubmitChanges();
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
            if (this.Surrendered)
            {
                // Reset flag
                this.Surrendered = false;
            }

            // Check if cargo was not picked up
            if (this.CargoJettisoned)
            {
                // Delete ignored space goods
                db.CombatGoods.DeleteAllOnSubmit(this.CombatGoods);

                // Reset flag
                this.CargoJettisoned = false;
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

            try
            {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to one of this combat record
                // Overwrite those changes
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
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
            
            // Reset jump drive charges
            this.ShipOther.CurrentJumpDriveCharge = 0;
            this.ShipTurn.CurrentJumpDriveCharge = 0;

            // Sending cargo into space
            this.SendCargoIntoSpace(this.ShipOther);

            // Move cargo into our cargo bays
            this.LoadCargo();

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
                this.CreditsLooted = otherPlayer.CashCredits;
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
                
                turnPlayer.ShipsDestroyed++;

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

                // Repair winners ship
                this.ShipTurn.Repair();
            }
            else
            {
                throw new NotImplementedException("Non-player ship or NPC combat not supported");
            }
        }

        /// <summary>
        /// Sends all the cargo on the ship into space.
        /// </summary>
        /// <param name="sourceShip">The source ship to throw the cargo out of.</param>
        private void SendCargoIntoSpace(Ship sourceShip)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // We will unload all the cargo off of the source ship and move it into the combat 'space'
            foreach (ShipGood shipGood in sourceShip.ShipGoods) 
            {
                CombatGood good = (from g in this.CombatGoods
                                   where g.Good == shipGood.Good
                                   select g).SingleOrDefault();
                if (good == null)
                {
                    good = new CombatGood();
                    good.Combat = this;
                    good.Good = shipGood.Good;
                    db.CombatGoods.InsertOnSubmit(good);
                }

                // Into space the good goes...
                good.Quantity += shipGood.Quantity;
                // The good is no longer on the ship
                shipGood.Quantity = 0;
            }

            // Update the player stats on lost cargo
            Player shipPlayer = sourceShip.Players.SingleOrDefault();
            if (shipPlayer != null)
            {
                int cargoWorth = this.CombatGoods.Sum(g => (g.Quantity * g.Good.BasePrice));
                shipPlayer.CargoLostWorth += cargoWorth;
            }

            try
            {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to the ship or combat goods
                // Keep our changes
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
        }

        /// <summary>
        /// Loads as much of the cargo in space as possible into the current turn ship.
        /// </summary>
        private void LoadCargo()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // We will unload all the cargo off of the source ship and move it into the combat 'space'
            // Load the higher priced goods first
            foreach (CombatGood good in this.CombatGoods.OrderByDescending(g => g.Good.BasePrice))
            {
                // Load up the cargo
                int quantityLoaded = this.ShipTurn.AddGood(good.GoodId, good.Quantity);
                good.QuantityPickedUp = quantityLoaded;
            }

            Logger.Write("Picked up cargo", "Model", 150, 0, TraceEventType.Verbose, "Combat.LoadCargo",
                new Dictionary<string, object>
                {
                    { "CombatId", this.CombatId },
                    { "TurnShipId", this.ShipTurn.ShipId },
                    { "OtherShipId", this.ShipOther.ShipId },
                    { "TotalCargoCount", this.CombatGoods.Sum(g => g.Quantity) },
                    { "TotalPickupCount", this.CombatGoods.Sum(g => g.QuantityPickedUp) }
                }
            );

            // Update the player stats on looted cargo
            Player shipPlayer = this.ShipTurn.Players.SingleOrDefault();
            if (shipPlayer != null)
            {
                int cargoWorth = this.CombatGoods.Sum(g => (g.Quantity * g.Good.BasePrice));
                shipPlayer.CargoLootedWorth += cargoWorth;
            }

            try
            {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to the ship or combat goods
                // Keep our changes
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
        }
    }
}
