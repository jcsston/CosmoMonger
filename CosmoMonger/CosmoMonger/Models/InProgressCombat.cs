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
        /// The default number of points give per turn
        /// </summary>
        public const int PointsPerTurn = 20;

        /// <summary>
        /// This is an list of Goods that can be picked up by the opposing player. 
        /// If the goods are not picked up, the goods are deleted from the system.
        /// </summary>
        public ShipGood[] CargoToPickup;

        /// <summary>
        /// A reference to the current Ship ('Player') turn. 
        /// We reference Ships rather than Players because NPCs are not considered Players, 
        /// but do own Ships and can fight.
        /// </summary>
        private Ship ShipTurn
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
        private Ship ShipOther
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
        }

        /// <summary>
        /// Gives up the rest of the turn and signals that the current ship is surrendering to the opposing ship.
        /// </summary>
        public void OfferSurrender()
        {

        }

        /// <summary>
        /// Accept the surrender of the opposing ship. 
        /// This gives all the goods and credits aboard the opposing ship to the current ship and ends combat.
        /// </summary>
        public void AcceptSurrender()
        {

        }

        /// <summary>
        /// Jettison all of the ships current cargo. 
        /// This will allow the ship to escape if the opposing ship picks up the cargo.
        /// </summary>
        public void JettisonShipCargo()
        {

        }

        /// <summary>
        /// Pickup cargo jettisoned by opposing ship, this will end combat as the other ship will escape. 
        /// If the cargo is not picked up, it is deleted on the next turn.
        /// </summary>
        public void PickupCargo()
        {

        }

        /// <summary>
        /// Uses the rest of the current turn points to charge the jump drive. 
        /// If the jump drive becomes completely charged, the ship escapes and combat is ended.
        /// </summary>
        public void ChargeJumpdrive()
        {

        }

        /// <summary>
        /// Ends the current ships turn, giving up any turn points left.
        /// </summary>
        public void EndTurn()
        {

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
