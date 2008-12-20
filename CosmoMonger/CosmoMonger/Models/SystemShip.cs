﻿namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        public void Buy(GameManager manager)
        {
            // Calcuate the total cost to the player
            int totalCost = this.Price - manager.CurrentPlayer.Ship.TradeInValue;

            // Check if the player has enough credits to buy the ship,
            // We check if the totalCost is postive as the player could make a profit by getting a smaller ship
            if (totalCost > 0 && manager.CurrentPlayer.CashCredits < totalCost)
            {
                throw new InvalidOperationException("Not enough credits to buy ship");
            }

            // Check if the new ship has enough cargo space to transfer everything 
            Ship currentShip = manager.CurrentPlayer.Ship;
            int cargoSpaceNeeded = currentShip.CargoSpaceTotal - currentShip.CargoSpaceFree;
            if (cargoSpaceNeeded > this.BaseShip.CargoSpace)
            {
                throw new InvalidOperationException("Not enough cargo space on new ship to transfer over cargo");
            }

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

            // Swap in the players new ship
            currentShip.BaseShip = this.BaseShip;

            // Commit changes to the database
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            db.SubmitChanges();
        }
    }
}