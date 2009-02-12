namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// Extends the partial LINQ NPC class.
    /// </summary>
    public partial class Npc
    {
        static private Random rnd = new Random();

        public virtual void DoAction()
        {
            Logger.Write("Enter Npc.DoAction blah", "NPC", 200, 0, TraceEventType.Verbose);
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
            this.NextActionTime = DateTime.Now.AddMinutes(5);
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

        /// <summary>
        /// Updates the system good price.
        /// </summary>
        private void UpdateSystemGoodPrice()
        {
            Logger.Write("Enter Npc.UpdateSystemGoodPrice", "NPC", 100, 0, TraceEventType.Verbose);
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Mark the next time the system good prices will need to be updated
            this.NextActionTime = DateTime.Now.AddMinutes(5);
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
                // By doing a slight random adjustment of the new price multipler by -/+ 0.10
                double rndPriceMultiplerAdjust = 0.10 - (rnd.NextDouble() / 5);
                newPriceMultipler += rndPriceMultiplerAdjust;

                // Limit the price multipler to between 0.33 and 3.0
                newPriceMultipler = Math.Max(0.33, newPriceMultipler);
                newPriceMultipler = Math.Min(3.0, newPriceMultipler);

                double oldPriceMultipler = good.PriceMultiplier;
                
                // Take average of previous and current price multipler
                good.PriceMultiplier = (good.PriceMultiplier + newPriceMultipler) / 2.0;

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
