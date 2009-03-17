//-----------------------------------------------------------------------
// <copyright file="CosmoSystem.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extension of the partial LINQ class System
    /// </summary>
    public partial class CosmoSystem
    {
        /// <summary>
        /// Returns the ships available for purchase in the system.
        /// </summary>
        /// <returns>Array of SystemShip available in the system</returns>
        public virtual SystemShip[] GetAvailableShips()
        {
            return (from ss in this.SystemShips
                    where ss.Quantity > 0
                    select ss).ToArray();
        }

        /// <summary>
        /// Fetches the SystemShip object for the passed in systemShip id.
        /// </summary>
        /// <param name="shipId">The base ship id of the SystemShip.</param>
        /// <returns>
        /// The SystemShip with the matching base ship id. 
        /// If the SystemShip doesn't exist, null is returned.
        /// </returns>
        public virtual SystemShip GetShip(int shipId)
        {
            return (from ss in this.SystemShips
                    where ss.BaseShipId == shipId
                    select ss).SingleOrDefault();
        }

        /// <summary>
        /// Returns the goods available for purchase in the system.
        /// </summary>
        /// <returns>Array of SystemGood in the system</returns>
        public virtual SystemGood[] GetGoods()
        {
            return this.SystemGoods.ToArray();
        }

        /// <summary>
        /// Fetches the SystemGood object for the passed in systemGood id. 
        /// </summary>
        /// <param name="goodId">The good id of the SystemGood object to get.</param>
        /// <returns>
        /// The SystemGood object with the matching goodId. 
        /// If there is no SystemGood for the passed in good id, null is returned.
        /// </returns>
        public virtual SystemGood GetGood(int goodId)
        {
            return (from sg in this.SystemGoods
                    where sg.GoodId == goodId
                    select sg).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the SystemEngineUpgrades objects for the System.
        /// </summary>
        /// <returns>Array of JumpDrive upgrades in the system</returns>
        public virtual SystemJumpDriveUpgrade[] GetEngineUpgrades()
        {
            return this.SystemJumpDriveUpgrades.ToArray();
        }

        /// <summary>
        /// Fetches the SystemEngineUpgrade object for the passed upgrade id. 
        /// Returns null if the SystemEngineUpgrade does not exist.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns>The matching Engine/JumpDrive upgrade for the system</returns>
        public virtual SystemJumpDriveUpgrade GetEngineUpgrade(int upgradeId)
        {
            return (from su in this.SystemJumpDriveUpgrades
                    where su.JumpDriveId == upgradeId
                    select su).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the SystemShieldUpgrades objects for the System.
        /// </summary>
        /// <returns>Array of Shield upgrades in the system</returns>
        public virtual SystemShieldUpgrade[] GetShieldUpgrades()
        {
            return this.SystemShieldUpgrades.ToArray();
        }

        /// <summary>
        /// Fetches the SystemShieldUpgrade object for the passed upgrade id. 
        /// Returns null if the SystemShieldUpgrade does not exist.
        /// </summary>
        /// <param name="shieldId">The shield id.</param>
        /// <returns>The SystemShieldUpgrade object matching the passed in shieldId</returns>
        public virtual SystemShieldUpgrade GetShieldUpgrade(int shieldId)
        {
            return (from su in this.SystemShieldUpgrades
                    where su.ShieldId == shieldId
                    select su).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the SystemWeaponUpgrades objects for the System.
        /// </summary>
        /// <returns>An array of Weapon upgrades in the system</returns>
        public virtual SystemWeaponUpgrade[] GetWeaponUpgrades()
        {
            return this.SystemWeaponUpgrades.ToArray();
        }

        /// <summary>
        /// Fetches the SystemWeaponUpgrade object for the passed upgrade id. 
        /// Returns null if the SystemWeaponUpgrade does not exist.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns>A SystemWeaponUpgrades</returns>
        public virtual SystemWeaponUpgrade GetWeaponUpgrade(int upgradeId)
        {
            return (from su in this.SystemWeaponUpgrades
                    where su.WeaponId == upgradeId
                    select su).SingleOrDefault();
        }

        /// <summary>
        /// Adds the good type to this system
        /// </summary>
        /// <param name="goodId">The good type to add to this system.</param>
        /// <param name="quantity">The quantity of the good to add.</param>
        public virtual void AddGood(int goodId, int quantity)
        {
            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "GoodId", goodId },
                { "Quantity", quantity },
                { "SystemId", this.SystemId }
            };
            Logger.Write("Adding Good to System in CosmoSystem.AddGood", "Model", 150, 0, TraceEventType.Verbose, "Adding Good to System", props);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            SystemGood systemGood = this.GetGood(goodId);
            if (systemGood == null)
            {
                // Add this new good type to the system
                systemGood = new SystemGood();
                systemGood.CosmoSystem = this;
                systemGood.GoodId = goodId;
                systemGood.PriceMultiplier = 1.0;
                systemGood.Demand = SystemGood.DemandType.Average;
                this.SystemGoods.Add(systemGood);
            }

            // Add the correct number of goods to the system
            systemGood.Quantity += quantity;

            // Save changes to the database
            db.SubmitChanges();
        }

        /// <summary>
        /// Gets the ships that are leaving the system.
        /// </summary>
        /// <returns>An array of Ship objects that are leaving the system</returns>
        public virtual IEnumerable<Ship> GetLeavingShips()
        {
            return (from s in this.Ships
                    where s.TargetSystemId.HasValue
                    && s.Players.Any(p => p.Alive)
                    && s.DamageHull < 100
                    select s).AsEnumerable();
        }

        /// <summary>
        /// Gets the ships that are currently in the system.
        /// </summary>
        /// <returns>An array of Ship objects that are in the system</returns>
        public virtual IEnumerable<Ship> GetShipsInSystem()
        {
            return (from s in this.Ships
                    where s.Players.Any(p => p.Alive)
                    && s.DamageHull < 100
                    select s).AsEnumerable();
        }
    }
}
