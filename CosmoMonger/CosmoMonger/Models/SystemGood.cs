//-----------------------------------------------------------------------
// <copyright file="SystemGood.cs" company="CosmoMonger">
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

    /// <summary>
    /// Extension of the partial LINQ class SystemGood
    /// </summary>
    public partial class SystemGood
    {
        /// <summary>
        /// Gets the actual price of the good.
        /// Calcuated by taking the Good.BasePrice and the SystemGood.PriceMultiplier
        /// </summary>
        public int Price
        {
            get
            {
                Debug.Assert(this.PriceMultiplier >= 0.25 && this.PriceMultiplier <= 4.0, "Prices cannot be below 0.25 and cannot be 4x+ the base price.");
                return (int)(this.Good.BasePrice * this.PriceMultiplier);
            }
        }

        /// <summary>
        /// Buys the specified quantity of goods from the system.
        /// </summary>
        /// <param name="manager">The current GameManager object.</param>
        /// <param name="quantity">The quantity of the good to buy.</param>
        public void Buy(GameManager manager, int quantity)
        {
            // Check that we are not trying to buy more goods than there is
            if (this.Quantity < quantity)
            {
                throw new ArgumentOutOfRangeException("quantity", quantity, "Unable to buy more goods than at the system");
            }

            // Check if the player has enough money to buy the goods
            int totalCost = (int)this.Price * quantity;
            if (manager.CurrentPlayer.CashCredits < totalCost)
            {
                throw new ArgumentException("Not enough credits to buy requested number of goods", "quantity");
            }
            
            // Check if the player has enough cargo space to carry the goods
            Ship playerShip = manager.CurrentPlayer.Ship;
            if (playerShip.CargoSpaceFree < quantity)
            {
                throw new ArgumentException("Not enough cargo space to carry requested number of goods", "quantity");
            }

            // Remove the goods from the system
            this.Quantity -= quantity;

            // We don't delete the SystemGood even if quantity is 0 because the SystemGood also signals if the system buys the good

            // Add the goods to the player ship
            ShipGood playerGood = (from sg in playerShip.ShipGoods
                                   where sg.Good == this.Good
                                   select sg).SingleOrDefault();
            if (playerGood == null)
            {
                // Ship is not already carrying this good, so we have to create a new ShipGood
                playerGood = new ShipGood();
                playerGood.Ship = playerShip;
                playerGood.Good = this.Good;
                playerGood.Quantity = quantity;
                playerShip.ShipGoods.Add(playerGood);
            }
            else
            {
                // Ship is already carrying this good, add to the existing ShipGood
                playerGood.Quantity += quantity;
            }

            // Charge the player for the goods
            manager.CurrentPlayer.CashCredits -= totalCost;

            // Commit changes to the database
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            db.SubmitChanges();
        }
    }
}
