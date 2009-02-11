//-----------------------------------------------------------------------
// <copyright file="InProgressCombat.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Linq;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// Extension of the partial LINQ class InProgressCombat.
    /// This class handles the combat conditions and scenerios.
    /// </summary>
    public partial class InProgressCombat
    {
        /// <summary>
        /// The default number of points given per turn
        /// </summary>
        public const int PointsPerTurn = 20;

        /// <summary>
        /// This is an list of Goods that can be picked up by the opposing player.
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
        /// A reference to the current Ship ('Player') turn.
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
        /// A reference to the other Ship.
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
        public void FireWeapon()
        {
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

            // Power up weapon
            // Damage should be at least 1, even with powerful shields
            double weaponDamage = Math.Max(firingWeapon.Power - shieldStrength, 1.0);

            // Apply damage
            // Half goes to shields and the rest goes to the hull
            // Max damage is 100%
            double newDamageShield = Math.Ceiling(this.ShipOther.DamageShield + (weaponDamage / 0.5));
            this.ShipOther.DamageShield = (int)Math.Min(newDamageShield, 100);
            
            double newDamageHull = this.ShipOther.DamageHull + ((weaponDamage / 1.5) * (this.ShipOther.DamageShield / 100.0));
            newDamageHull = Math.Ceiling(newDamageHull);
            this.ShipOther.DamageHull = (int)Math.Min(newDamageHull, 100);

            // Deduct turn points
            this.TurnPointsLeft -= firingWeapon.TurnCost;

            try
            {
                // Save database changes
                db.SubmitChanges();
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
                // Automaticly end this players turn
                this.EndTurn();
            }

            if (this.TurnPointsLeft <= 0)
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
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Surrender flag must be not set
            if (this.Surrender)
            {
                throw new InvalidOperationException("Other ship already offered surrender");
            }

            this.Surrender = true;

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
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Surrender flag must be set
            if (!this.Surrender)
            {
                throw new InvalidOperationException("No surrender offered");
            }

            // Move enemy cargo into our cargo bays
            foreach (ShipGood cargo in this.ShipOther.ShipGoods)
            {
                this.ShipTurn.AddGood(cargo.GoodId, cargo.Quantity);
            }

            // Delete the cargo from the other ship
            db.ShipGoods.DeleteAllOnSubmit(this.ShipOther.ShipGoods);

            db.SubmitChanges();

            // Combat is ended
            this.End();
        }

        /// <summary>
        /// Jettison all of the ships current cargo. 
        /// This will allow the ship to escape if the opposing ship picks up the cargo.
        /// </summary>
        public void JettisonShipCargo()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Jettison Cargo flag must be not set
            if (this.JettisonCargo)
            {
                throw new InvalidOperationException("There is already cargo jettisoned");
            }

            this.JettisonCargo = true;

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

            db.SubmitChanges();

            // Combat is ended
            this.End();
        }

        /// <summary>
        /// Uses the rest of the current turn points to charge the jump drive. 
        /// If the jump drive becomes completely charged, the ship escapes and combat is ended.
        /// </summary>
        public void ChargeJumpdrive()
        {
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
            }

            db.SubmitChanges();

            this.SwapTurn();
        }

        /// <summary>
        /// Swaps the turn to the other ship, giving up any turn points left.
        /// This does not check the surrender or cargo flags.
        /// </summary>
        private void SwapTurn()
        {
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
            this.TurnPointsLeft = InProgressCombat.PointsPerTurn;

            db.SubmitChanges();
        }

        /// <summary>
        /// Ends the current ships turn. Giving control to the other ship.
        /// This does check the surrender and cargo flags.
        /// </summary>
        public void EndTurn()
        {
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
        /// Ends this combat.
        /// </summary>
        public void End()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            db.InProgressCombats.DeleteOnSubmit(this);
            db.SubmitChanges();
        }
    }
}
