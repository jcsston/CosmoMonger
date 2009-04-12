//-----------------------------------------------------------------------
// <copyright file="SystemJumpDriveUpgrade.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
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
    /// Extension of the partial LINQ class SystemJumpDriveUpgrade
    /// </summary>
    public partial class SystemJumpDriveUpgrade 
    {
        /// <summary>
        /// Gets the price per ship level of the jumpdrive.
        /// Calcuated by taking the JumpDrive.BasePrice and the SystemJumpDriveUpgrade.PriceMultiplier
        /// </summary>
        public int PricePerLevel
        {
            get
            {
                return (int)(this.JumpDrive.BasePrice * this.PriceMultiplier);
            }
        }

        /// <summary>
        /// Gets the current price for this upgrade, not counting trade-in.
        /// </summary>
        /// <param name="currentShip">The current ship.</param>
        /// <returns>The total cost </returns>
        public virtual int GetPrice(Ship currentShip)
        {
            return (int)(this.PricePerLevel * currentShip.BaseShip.Level);
        }

        /// <summary>
        /// Buys this JumpDrive update for the passed in ship
        /// </summary>
        /// <param name="currentShip">The Ship object to use for this transaction.</param>
        /// <exception cref="InvalidOperationException">Thrown when not enough credits to buy the new upgrade or not enough cargo space to install the new upgrade</exception>
        public virtual void Buy(Ship currentShip)
        {
            // Calcuate the total cost to the player
            int totalCost = this.GetPrice(currentShip) - currentShip.JumpDrive.GetTradeInValue(currentShip);

            // Check if the player has enough credits to buy the upgrade,
            // We check if the totalCost is postive as the player could make a profit by getting a smaller ship
            if (totalCost > 0 && currentShip.Credits < totalCost)
            {
                throw new InvalidOperationException("Not enough credits to buy JumpDrive upgrade");
            }

            // Check if the new ship has enough cargo space to transfer everything 
            int cargoSpaceNeeded = this.JumpDrive.CargoCost - currentShip.JumpDrive.CargoCost;
            if (cargoSpaceNeeded > currentShip.CargoSpaceFree)
            {
                throw new InvalidOperationException("Not enough cargo space on ship to install JumpDrive upgrade");
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "ShipId", currentShip.ShipId },
                { "OldJumpDriveId", currentShip.JumpDriveId },
                { "NewJumpDriveId", this.JumpDriveId },
                { "CargoSpaceNeeded", cargoSpaceNeeded },
                { "TotalCost", totalCost }
            };
            Logger.Write("Buying new JumpDrive upgrade", "Model", 500, 0, TraceEventType.Information, "SystemJumpDriveUpgrade.Buy", props);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // 'Trade-in' the current upgrade
            SystemJumpDriveUpgrade tradeInUpgrade = (from ju in this.CosmoSystem.SystemJumpDriveUpgrades
                                                     where ju.JumpDrive == currentShip.JumpDrive
                                                     select ju).SingleOrDefault();
            if (tradeInUpgrade == null)
            {
                // The players trade-in isn't currently sold in the system, so we have to add it
                tradeInUpgrade = new SystemJumpDriveUpgrade();
                tradeInUpgrade.JumpDrive = currentShip.JumpDrive;
                tradeInUpgrade.CosmoSystem = this.CosmoSystem;
                tradeInUpgrade.Quantity = 1;
                this.CosmoSystem.SystemJumpDriveUpgrades.Add(tradeInUpgrade);
            }
            else
            {
                // The players trade-in is currently sold in the system, so add it to collection sold
                tradeInUpgrade.Quantity += 1;
            }

            // Remove this new upgrade from the pool
            this.Quantity -= 1;

            // Charge the player
            currentShip.Credits -= totalCost;

            // Swap in the players new upgrade
            currentShip.JumpDrive = this.JumpDrive;

            // Commit changes to the database
            db.SaveChanges();
        }
    }
}
