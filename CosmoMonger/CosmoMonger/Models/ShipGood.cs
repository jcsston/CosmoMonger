//-----------------------------------------------------------------------
// <copyright file="ShipGood.cs" company="CosmoMonger">
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
    /// Extension of the partial LINQ class ShipGood
    /// </summary>
    public partial class ShipGood
    {
        /// <summary>
        /// Sells the specified quantity of goods off the ship.
        /// </summary>
        /// <param name="manager">The current GameManager object.</param>
        /// <param name="quantity">The quantity of goods to sell.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when trying to sell more goods than avaiable on the ship.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the good is not bought in the current system.</exception>
        public virtual void Sell(GameManager manager, int quantity)
        {
            // Check the the good is actually sold/bought in the current system
            SystemGood sellingGood = this.Ship.CosmoSystem.GetGood(this.GoodId);
            if (sellingGood == null)
            {
                throw new InvalidOperationException("This good is not sold/bought in the current system");
            }

            // Check that we are not trying to sell more goods than we have
            if (this.Quantity < quantity)
            {
                throw new ArgumentOutOfRangeException("quantity", quantity, "Unable to sell more goods than aboard");
            }

            // Calcuate how much we will make selling these goods
            int profit = quantity * sellingGood.Price;

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "PlayerId", manager.CurrentPlayer.PlayerId },
                { "ShipId", this.ShipId },
                { "GoodId", this.GoodId },
                { "Quantity", quantity },
                { "Profit", profit }
            };
            Logger.Write("Selling goods in ShipGood.Sell", "Model", 500, 0, TraceEventType.Information, "Selling Goods", props);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Transfer the good(s) to the system
            sellingGood.Quantity += quantity;

            try
            {
                // Commit changes to the database
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to the SystemGood row, 
                // which could be from the number of goods or the price changing 
                // Best case to toss our changes and try to sell the good again
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.OverwriteCurrentValues);
                }

                // This does have the chance of a stack overflow, we should find out in testing
                this.Sell(manager, quantity);
                return;
            }

            // Remove good(s) from the ship
            this.Quantity -= quantity;
            if (this.Quantity == 0)
            {
                // TODO: Should we delete this object?
            }

            // Add to the players cash credits account
            manager.CurrentPlayer.CashCredits += profit;

            try
            {
                // Commit changes to the database
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to the players record, this is invalid
                // and so we use our values and ignore the new data
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    // Refresh current values from database
                    occ.Resolve(RefreshMode.KeepCurrentValues);
                }
            }
        }
    }
}
