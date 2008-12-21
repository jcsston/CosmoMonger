//-----------------------------------------------------------------------
// <copyright file="ShipGood.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension of the partial LINQ class ShipGood
    /// </summary>
    public partial class ShipGood
    {
        /// <summary>
        /// Sells the specified quantity of goods off the ship.
        /// If the good is not sold in the current system, an InvalidOperationException exception is thrown.
        /// </summary>
        /// <param name="manager">The current GameManager object.</param>
        /// <param name="quantity">The quantity of goods to sell, if there are not enough goods, an ArgumentOutOfRangeException is thrown.</param>
        public void Sell(GameManager manager, int quantity)
        {
            // Check the the good is actually sold/bought in the current system
            SystemGood sellingGood = this.Ship.CosmoSystem.GetGood(this.GoodId);
            if (sellingGood == null)
            {
                throw new InvalidOperationException("This good is not sold/bought in the current system");
            }

            // Check that we are not trying to sell more goods than we have
            if (this.Quantity > quantity)
            {
                throw new ArgumentOutOfRangeException("quantity", quantity, "Unable to sell more goods than aboard");
            }

            // Calcuate how much we will make selling these goods
            int profit = quantity * sellingGood.Price;
            
            // Remove good(s) from the ship
            this.Quantity -= quantity;
            if (this.Quantity == 0)
            {
                // TODO: Should we delete this object?
            }

            // Transfer the good(s) to the system
            sellingGood.Quantity += quantity;

            // Add to the players cash credits account
            manager.CurrentPlayer.CashCredits += profit;
            
            // Commits changes to the database
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            db.SubmitChanges();
        }
    }
}
