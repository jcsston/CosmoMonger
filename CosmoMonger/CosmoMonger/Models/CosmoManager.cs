//-----------------------------------------------------------------------
// <copyright file="CosmoManager.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This is the central control class for the CosmoMonger game.
    /// </summary>
    public class CosmoManager
    {
        /// <summary>
        /// Gets the CosmoMonger db context.
        /// </summary>
        /// <returns>LINQ CosmoMongerDbDataContext object</returns>
        public static CosmoMongerDbDataContext GetDbContext()
        {
            return Utility.DataContextFactory.GetScopedDataContext<CosmoMongerDbDataContext>("CosmoMonger", ConfigurationManager.ConnectionStrings["CosmoMongerConnectionString"].ConnectionString);
        }

        /// <summary>
        /// Gets the current code version. Ex. Subversion Revision number
        /// </summary>
        /// <returns>Revision number of running code</returns>
        public static int GetCodeVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.Revision;
        }

        /// <summary>
        /// Gets the database version. Ex. The liquibase changelog number
        /// </summary>
        /// <returns>Database version of connected database</returns>
        public static int GetDatabaseVersion()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return db.ExecuteQuery<int>("SELECT MAX(CAST(Id AS int)) FROM DATABASECHANGELOG WHERE ISNUMERIC(Id) = 1").Single();
        }

        /// <summary>
        /// Calls DoAction on all NPCs in the galaxy. 
        /// This method will be called every 5 seconds via Cache Expirations to keep the NPCs 
        /// busy in the galaxy even when no human players are hitting pages.
        /// </summary>
        public void DoPendingNPCActions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the system good count, causing systems to produce goods as needed.
        /// </summary>
        public void UpdateSystemGoodCount()
        {
            Random rnd = new Random();
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            foreach (Good good in db.Goods)
            {
                // Get the total number of this good type avaiable in all systems
                int totalSystemGoodCount = good.SystemGoods.Sum(x => x.Quantity);

                // Check if we need to add some of this good to the galaxy
                if (totalSystemGoodCount < good.TargetCount)
                {
                    // Randomly select a good at a system to produce
                    var goodProducingSystems = (from g in good.SystemGoods
                                                where g.ProductionFactor > 0
                                                select g);
                    if (goodProducingSystems.Count() == 0)
                    {
                        // No systems produce this good?
                        // Continue on to the next good type
                        continue;
                    }

                    SystemGood selectedProducingSystemGood = goodProducingSystems.ElementAt(rnd.Next(goodProducingSystems.Count()));

                    // Produce the good, using the count needed and the production factor
                    double adjustedProductionFactor = (rnd.NextDouble() + selectedProducingSystemGood.ProductionFactor) / 2;
                    int lackingGoodCount = (int)(rnd.Next(10) * adjustedProductionFactor);
                    selectedProducingSystemGood.Quantity += lackingGoodCount;

                    // Send changes to the database
                    db.SubmitChanges();
                }

                // Now consume some of this good in the galaxy
                // Randomly select a good at a system to produce
                var goodConsumingSystems = (from g in good.SystemGoods
                                            where g.ConsumptionFactor > 0
                                            && g.Quantity > 0
                                            select g);
                if (goodConsumingSystems.Count() == 0)
                {
                    // No systems consume this good?
                    // Continue on to the next good type
                    continue;
                }

                SystemGood selectedConsumingSystemGood = goodConsumingSystems.ElementAt(rnd.Next(goodConsumingSystems.Count()));

                // Consuming the good, using the count needed and the consumption factor
                double adjustedConsumptionFactor = (rnd.NextDouble() + selectedConsumingSystemGood.ConsumptionFactor) / 2;
                int usageGoodCount = (int)(rnd.Next(10) * adjustedConsumptionFactor);
                selectedConsumingSystemGood.Quantity -= Math.Min(usageGoodCount, selectedConsumingSystemGood.Quantity);

                // Send changes to the database
                db.SubmitChanges();
            }
        }

        /// <summary>
        /// Updates the system good price.
        /// </summary>
        public void UpdateSystemGoodPrice()
        {
            Random rnd = new Random();
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            foreach (SystemGood good in db.SystemGoods)
            {
                // Get the total number of this good type avaiable in all systems
                int targetTotal = good.Good.TargetCount;
                int systemsWithGood = (from s in good.Good.SystemGoods
                                       where s.Quantity > 0
                                       select s).Count();
                int currentSystemGoodCount = Math.Max(good.Quantity, 1);
                double newPriceMultipler = (1.0 * targetTotal / systemsWithGood) / currentSystemGoodCount;

                // Give a little bit of randomization to the price multipler
                newPriceMultipler = newPriceMultipler * (rnd.NextDouble() + 0.5);

                // Limit the price multipler to between 0.5 and 3.0
                newPriceMultipler = Math.Max(0.25, newPriceMultipler);
                newPriceMultipler = Math.Min(4.0, newPriceMultipler);

                // Take average of previous and current price multipler
                good.PriceMultiplier = (good.PriceMultiplier + newPriceMultipler) / 2.0;

                // Send changes to database
                db.SubmitChanges();
            }
        }
    }
}
