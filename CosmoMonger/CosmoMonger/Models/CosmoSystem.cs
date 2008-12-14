﻿namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension of the partial LINQ class System
    /// </summary>
    public partial class CosmoSystem
    {
        /// <summary>
        /// Returns the ships available for purchase in the system.
        /// </summary>
        /// <returns>Array of SystemShip available in the system</returns>
        public SystemShip[] GetAvailableShips()
        {
            return this.SystemShips.ToArray();
        }

        /// <summary>
        /// Fetches the SystemShip object for the passed in systemShip id. If the SystemShip doesn't exist, null is returned.
        /// </summary>
        /// <param name="systemShipId">The base ship id of the SystemShip.</param>
        /// <returns>The SystemShip with the matching base ship id</returns>
        public SystemShip GetShip(int systemShipId)
        {
            return (from ss in this.SystemShips
                    where ss.BaseShipId == systemShipId
                    select ss).SingleOrDefault();
        }

        /// <summary>
        /// Returns the goods available for purchase in the system.
        /// </summary>
        /// <returns>Array of SystemGood in the system</returns>
        public SystemGood[] GetGoods()
        {
            return this.SystemGoods.ToArray();
        }

        /// <summary>
        /// Fetches the SystemGood object for the passed in systemGood id. If the SystemGood doesn't exist, null is returned.
        /// </summary>
        /// <param name="systemGoodId">The system good id.</param>
        /// <returns>Matching Good object</returns>
        public SystemGood GetGood(int systemGoodId)
        {
            return (from sg in this.SystemGoods
                    where sg.GoodId == systemGoodId
                    select sg).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the SystemEngineUpgrades objects for the System.
        /// </summary>
        /// <returns></returns>
        public SystemJumpDriveUpgrade[] GetEngineUpgrades()
        {
            return this.SystemJumpDriveUpgrades.ToArray();
        }

        /// <summary>
        /// Fetches the SystemEngineUpgrade object for the passed upgrade id. 
        /// Returns null if the SystemEngineUpgrade does not exist.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns></returns>
        public SystemJumpDriveUpgrade GetEngineUpgrade(int upgradeId)
        {
            return (from su in this.SystemJumpDriveUpgrades
                    where su.JumpDriveId == upgradeId
                    select su).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the SystemShieldUpgrades objects for the System.
        /// </summary>
        /// <returns></returns>
        public SystemShieldUpgrade[] GetShieldUpgrades()
        {
            return this.SystemShieldUpgrades.ToArray();
        }

        /// <summary>
        /// Fetches the SystemShieldUpgrade object for the passed upgrade id. 
        /// Returns null if the SystemShieldUpgrade does not exist.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns></returns>
        public SystemShieldUpgrade GetShieldUpgrade(int upgradeId)
        {
            return (from su in this.SystemShieldUpgrades
                    where su.ShieldId == upgradeId
                    select su).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the SystemWeaponUpgrades objects for the System.
        /// </summary>
        /// <returns></returns>
        public SystemWeaponUpgrade[] GetWeaponUpgrades()
        {
            return this.SystemWeaponUpgrades.ToArray();
        }

        /// <summary>
        /// Fetches the SystemWeaponUpgrade object for the passed upgrade id. Returns null if the SystemWeaponUpgrade does not exist.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns></returns>
        public SystemWeaponUpgrade GetWeaponUpgrade(int upgradeId)
        {
            return (from su in this.SystemWeaponUpgrades
                    where su.WeaponId == upgradeId
                    select su).SingleOrDefault();
        }
    }
}
