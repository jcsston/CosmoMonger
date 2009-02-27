//-----------------------------------------------------------------------
// <copyright file="Npc.cs" company="CosmoMonger">
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
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extends the partial LINQ NPC class.
    /// </summary>
    public partial class Npc
    {
        /// <summary>
        /// Constant for the number of minutes to schedule between system good price and quanitity updates.
        /// </summary>
        public const int MinutesBetweenSystemGoodUpdates = 5;

        /// <summary>
        /// A persitant random number generator for Npc code.
        /// </summary>
        private static Random rnd = new Random();

        public virtual void DoAction()
        {
            Logger.Write("Enter Npc.DoAction", "NPC", 200, 0, TraceEventType.Verbose);
            switch (this.NpcTypeId)
            {
                case 1:
                    // Special system good count NPC
                    this.UpdateSystemGoodCount();
                    break;
                case 2:
                    // Special system good price NPC
                    this.UpdateSystemGoodPrice();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("NpcTypeId", this.NpcTypeId, "Invalid NPC Type");
            }
        }

        /// <summary>
        /// Updates the system good count, causing systems to produce goods as needed.
        /// </summary>
        private void UpdateSystemGoodCount()
        {
            Logger.Write("Enter Npc.UpdateSystemGoodCount", "NPC", 100, 0, TraceEventType.Verbose);
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Mark the next time the system good count will need to be updated
            this.NextActionTime = DateTime.Now.AddMinutes(Npc.MinutesBetweenSystemGoodUpdates);
            try
            {
                // Send changes to database
                db.SubmitChanges();
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to this Npc object, 
                // which means another thread has already started recalculating the good prices.
                // We shouldn't redo that work, and so we exit
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    // Refresh current values from database
                    occ.Resolve(RefreshMode.OverwriteCurrentValues);
                }
                return;
            }

            foreach (Good good in db.Goods)
            {
                // Get the total number of this good type available in all systems
                int totalSystemGoodCount = good.SystemGoods.Sum(x => x.Quantity);

                // Check if we need to add some of this good to the galaxy
                while (totalSystemGoodCount < good.TargetCount)
                {
                    // Randomly select a good at a system to produce
                    var goodProducingSystems = (from g in good.SystemGoods
                                                where g.ProductionFactor > 0
                                                select g);
                    if (goodProducingSystems.Count() == 0)
                    {
                        // No systems produce this good?
                        // Continue on to the next good type
                        break;
                    }

                    SystemGood selectedProducingSystemGood = goodProducingSystems.ElementAt(rnd.Next(goodProducingSystems.Count()));

                    // Produce the good, using the count needed and the production factor
                    double adjustedProductionFactor = (rnd.NextDouble() + selectedProducingSystemGood.ProductionFactor) / 2;
                    int lackingGoodCount = (int)(rnd.Next(10) * adjustedProductionFactor);
                    selectedProducingSystemGood.Quantity += lackingGoodCount;


                    Logger.Write("Producing Goods", "NPC", 500, 0, TraceEventType.Verbose, "Producing Goods",
                        new Dictionary<string, object>
                        {
                            { "SystemId", selectedProducingSystemGood.SystemId },
                            { "GoodId", selectedProducingSystemGood.GoodId },
                            { "Quantity", selectedProducingSystemGood.Quantity },
                            { "AddedQuantity", lackingGoodCount }
                        }
                    );

                    // Update the total good count
                    totalSystemGoodCount += lackingGoodCount;

                    try
                    {
                        // Send changes to database
                        db.SubmitChanges();
                    }
                    catch (ChangeConflictException ex)
                    {
                        ExceptionPolicy.HandleException(ex, "SQL Policy");

                        // Another thread has made changes to this SystemGood row, 
                        // which could be from someone buying or selling the good at a system
                        // Best case to resolve this would be to simply start over in the good production,
                        // because the good quantity has been changed by another method.
                        foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                        {
                            // Refresh values from database
                            occ.Resolve(RefreshMode.OverwriteCurrentValues);
                        }
                        continue;
                    }
                }

                // Now consume some of this good in the galaxy
                // Randomly select a good at a system to consume
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
                usageGoodCount = Math.Min(usageGoodCount, selectedConsumingSystemGood.Quantity);
                selectedConsumingSystemGood.Quantity -= usageGoodCount;

                Logger.Write("Consuming Goods", "NPC", 500, 0, TraceEventType.Verbose, "Consuming Goods",
                    new Dictionary<string, object>
                    {
                        { "SystemId", selectedConsumingSystemGood.SystemId },
                        { "GoodId", selectedConsumingSystemGood.GoodId },
                        { "Quantity", selectedConsumingSystemGood.Quantity },
                        { "RemovedQuantity", usageGoodCount }
                    }
                );

                try
                {
                    // Send changes to database
                    db.SubmitChanges();
                }
                catch (ChangeConflictException ex)
                {
                    ExceptionPolicy.HandleException(ex, "SQL Policy");

                    // Another thread has made changes to this SystemGood row, 
                    // which could be from someone buying or selling the good at a system
                    // Best case to resolve this would be to simply ignore the good consumption,
                    // we like to produce more than we consume
                    foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                    {
                        // Refresh values from database
                        occ.Resolve(RefreshMode.OverwriteCurrentValues);
                    }
                }
            }
        }

        public double CalculatePriceMultipler(int targetTotal, int systemsWithGood, int currentSystemGoodCount)
        {
            double newPriceMultipler;
            int min;
            int max;
            //int targetTotalLocal = targetTotal;


            //the following if else statements make the priceMultipler inversely related to the quantity.
            //the higher the quantity in a system, the lower the price (and vice versa)
            if (currentSystemGoodCount <= 1)
            {
                min = 276;
                max = 300;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 2) || (currentSystemGoodCount == 3))
            {
                min = 251;
                max = 275;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 4) || (currentSystemGoodCount == 5))
            {
                min = 226;
                max = 250;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 6) || (currentSystemGoodCount == 7))
            {
                min = 201;
                max = 225;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 8) || (currentSystemGoodCount == 9))
            {
                min = 176;
                max = 200;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 12) || (currentSystemGoodCount == 13))
            {
                min = 101;
                max = 125;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 14) || (currentSystemGoodCount == 15))
            {
                min = 76;
                max = 100;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 16) || (currentSystemGoodCount == 17))
            {
                min = 61;
                max = 75;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if ((currentSystemGoodCount == 18) || (currentSystemGoodCount == 19))
            {
                min = 46;
                max = 60;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else if (currentSystemGoodCount >= 20)
            {
                min = 33;
                max = 45;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }
            else //either currentSystemGoodCount is 10 or 11 or there is a problem
            {
                min = 126;
                max = 175;
                newPriceMultipler = (double)(rnd.Next(min, max) / 100.0);
            }

            return newPriceMultipler;
        }

        public double CalculatePriceMultiplerOld(int targetTotal, int systemsWithGood, int currentSystemGoodCount)
        {
            currentSystemGoodCount = Math.Max(currentSystemGoodCount, 1);
            double newPriceMultipler = (1.0 * targetTotal / systemsWithGood) / currentSystemGoodCount;

            // Limit the price multipler to between 0.33 and 3.0
            newPriceMultipler = Math.Max(0.33, newPriceMultipler);
            newPriceMultipler = Math.Min(3.0, newPriceMultipler);

            // Give a little bit of randomization to the price multipler
            // By doing a slight random adjustment of the new price multipler by -/+ 0.10
            double rndPriceMultiplerAdjust = 0.10 - (rnd.NextDouble() / 5);
            newPriceMultipler += rndPriceMultiplerAdjust;

            // Limit the price multipler to between 0.33 and 3.0
            newPriceMultipler = Math.Max(0.33, newPriceMultipler);
            newPriceMultipler = Math.Min(3.0, newPriceMultipler);

            return newPriceMultipler;
        }

        /// <summary>
        /// Updates the system good price.
        /// </summary>
        private void UpdateSystemGoodPrice()
        {
            Logger.Write("Enter Npc.UpdateSystemGoodPrice", "NPC", 100, 0, TraceEventType.Verbose);
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            //begin commenting out if I don't want to wait
            
            // Mark the next time the system good prices will need to be updated
            this.NextActionTime = DateTime.Now.AddMinutes(Npc.MinutesBetweenSystemGoodUpdates);
            try
            {
                // Send changes to database
                db.SubmitChanges();
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to this Npc object, 
                // which means another thread has already started recalculating the good prices.
                // We shouldn't redo that work, and so we exit
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    // Refresh current values from database
                    occ.Resolve(RefreshMode.OverwriteCurrentValues);
                }
                return;
            }
           
            //end commenting out 
            foreach (SystemGood good in db.SystemGoods)
            {
                // Get the total number of this good type avaiable in all systems
                int targetTotal = good.Good.TargetCount;
                int systemsWithGood = (from s in good.Good.SystemGoods
                                       where s.Quantity > 0
                                       select s).Count();
                double newPriceMultipler;

                // pre 2-21-09 way
                //newPriceMultipler = this.CalculatePriceMultipler(targetTotal, systemsWithGood, good.Quantity);

                //new way post 2-21-09
                newPriceMultipler = this.CalculatePriceMultipler(targetTotal, systemsWithGood, good.Quantity);

                double oldPriceMultipler = good.PriceMultiplier;
                // Take average of previous and current price multipler
                good.PriceMultiplier = (oldPriceMultipler + newPriceMultipler) / 2.0;

                Logger.Write("Adjusting Good Price", "NPC", 500, 0, TraceEventType.Verbose, "Adjusting Good Price",
                    new Dictionary<string, object>
                        {
                            { "SystemId", good.SystemId },
                            { "GoodId", good.GoodId },
                            { "PriceMultiplier", good.PriceMultiplier },
                            { "NewPriceMultipler", newPriceMultipler },
                            { "OldPriceMultipler", oldPriceMultipler }
                        }
                );

                try
                {
                    // Send changes to database
                    db.SubmitChanges();
                }
                catch (ChangeConflictException ex)
                {
                    ExceptionPolicy.HandleException(ex, "SQL Policy");

                    // Another thread has made changes to this SystemGood row, 
                    // which could be from someone buying or selling the good at a system
                    // Best case to resolve this would be to simply start over in the price calculatation,
                    // the previous price no longer valid.
                    foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                    {
                        // Refresh current values from database
                        occ.Resolve(RefreshMode.OverwriteCurrentValues);
                    }
                    continue;
                }
            }
        }
    }
}
