//-----------------------------------------------------------------------
// <copyright file="SystemShip.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
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
    /// Extension of the partial LINQ class SystemShip
    /// </summary>
    public partial class SystemShip
    {
        /// <summary>
        /// Gets the total price of the ship.
        /// Calcuated by taking the BaseShip.BasePrice and the SystemShip.PriceMultiplier
        /// </summary>
        public int Price
        {
            get
            {
                return (int)(this.BaseShip.BasePrice * this.PriceMultiplier);
            }
        }

        /// <summary>
        /// Buys this ship.
        /// </summary>
        /// <param name="currentShip">The Ship object to use for this transaction.</param>
        /// <exception cref="InvalidOperationException">Thrown when not enough credits to buy the new ship or not enough cargo space to hold current goods in new ship</exception>
        public virtual void Buy(Ship currentShip)
        {
            // Calcuate the total cost to the player
            int totalCost = this.Price - currentShip.TradeInValue;

            // Check if the player has enough credits to buy the ship,
            // We check if the totalCost is postive as the player could make a profit by getting a smaller ship
            if (totalCost > 0 && currentShip.Credits < totalCost)
            {
                throw new InvalidOperationException("Not enough credits to buy ship");
            }

            // Check if the new ship has enough cargo space to transfer everything 
            int cargoSpaceNeeded = currentShip.CargoSpaceTotal - currentShip.CargoSpaceFree;
            if (cargoSpaceNeeded > this.BaseShip.CargoSpace)
            {
                throw new InvalidOperationException("Not enough cargo space on new ship to transfer over cargo");
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "ShipId", currentShip.ShipId },
                { "OldShipBaseId", currentShip.BaseShipId },
                { "NewShipBaseId", this.BaseShipId }
            };
            Logger.Write("Buying new ship in SystemShip.Buy", "Model", 500, 0, TraceEventType.Information, "Buying Ship", props);

            // 'Trade-in' the current ship
            SystemShip tradeInShip = (from ss in this.CosmoSystem.SystemShips
                                      where ss.BaseShip == currentShip.BaseShip
                                      select ss).SingleOrDefault();
            if (tradeInShip == null)
            {
                // The players trade-in isn't currently sold in the system, so we have to add it
                tradeInShip = new SystemShip();
                tradeInShip.BaseShip = currentShip.BaseShip;
                tradeInShip.CosmoSystem = this.CosmoSystem;
                tradeInShip.Quantity = 1;
                this.CosmoSystem.SystemShips.Add(tradeInShip);
            }
            else
            {
                // The players trade-in is currently sold in the system, so add it to collection sold
                tradeInShip.Quantity += 1;
            }

            // Remove this new ship from the pool
            this.Quantity -= 1;

            // Charge the player
            currentShip.Credits -= totalCost;

            // Swap in the players new ship
            currentShip.BaseShip = this.BaseShip;

            // Swap in the new equipment
            currentShip.JumpDrive = this.BaseShip.InitialJumpDrive;
            currentShip.Shield = this.BaseShip.InitialShield;
            currentShip.Weapon = this.BaseShip.InitialWeapon;

            // Commit changes to the database
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            db.SubmitChanges();
        }
    }
}
