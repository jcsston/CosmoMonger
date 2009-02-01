namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extends the partial LINQ NPC class.
    /// </summary>
    public partial class Npc
    {
        public virtual void DoAction()
        {
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
            Logger.Write("Enter Npc.UpdateSystemGoodCount", "Model", 100, 0, TraceEventType.Verbose);
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            
            // Check if it has been long enough since the last good count update
            TimeSpan timeDelay = DateTime.Now - this.LastActionTime;
            if (timeDelay.TotalMinutes < 5)
            {
                return;
            }

            this.LastActionTime = DateTime.Now;
            db.SubmitChanges();

            Random rnd = new Random();
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
        private void UpdateSystemGoodPrice()
        {
            Logger.Write("Enter Npc.UpdateSystemGoodPrice", "Model", 100, 0, TraceEventType.Verbose);
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Check if it has been long enough since the last good count update
            TimeSpan timeDelay = DateTime.Now - this.LastActionTime;
            if (timeDelay.TotalMinutes < 5)
            {
                return;
            }

            this.LastActionTime = DateTime.Now;
            db.SubmitChanges();

            Random rnd = new Random();
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
