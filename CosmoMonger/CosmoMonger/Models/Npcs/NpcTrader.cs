//-----------------------------------------------------------------------
// <copyright file="NpcTrader.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Roger Boykin</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Web;
    using CosmoMonger.Models.Utility;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Contains logic for NPC Traders
    /// </summary>
    public class NpcTrader : NpcShipBase
    {
        /// <summary>
        /// This is the number we multiply times the new trader's ship's number of cargo spaces
        /// to provide the new trader with some credits to buy cargo.
        /// 5-3-09 was 400
        /// </summary>
        public const int BaseCreditMultiplier = 600;

        /// <summary>
        /// Used to slow down NPC traders.  This is the delay between ariving
        /// in systems and departing.
        /// </summary>
        public static TimeSpan DelayBeforeNextTravel = new TimeSpan(0, 2, 0);
        
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcTrader"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcTrader(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Setup the new Trader in the database (with a ship).
        /// </summary>
        public override void Setup()
        {
            // Call NpcShipBase class setup method
            // After this is where you would place more setup code
            base.Setup();
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        public override void DoAction()
        {
            if (!this.SetNextActionDelay(NpcTrader.DelayBeforeNextAction))
            {
                return;
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            Ship npcShip = this.npcRow.Ship;

            // Check if we are still traveling
            npcShip.CheckIfTraveling();

            // Check if we are currently in combat
            if (npcShip.InProgressCombat != null)
            {
                // In Combat!
                this.DoCombat();
            }
            else if (this.npcRow.NextTravelTime < DateTime.UtcNow)
            {
                // Create an array of goods that the Trader has onboard
                ShipGood[] goods = npcShip.GetGoods().Where(g => g.Quantity > 0).ToArray();

                // goodCount != 0, sell all the trader's goods 
                foreach (ShipGood good in goods)
                {
                    // Get the number of this good type onboard the Trader's ship
                    int shipGoodQuantity = good.Quantity;

                    // Find the price of the good
                    int shipGoodPrice = 0;

                    try
                    {
                        good.Sell(npcShip, shipGoodQuantity, shipGoodPrice);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "NPC Policy");
                    }
                }

                // This is the minimum amount of money a trader should have before buying goods
                int baseCreditsSizeAdjusted = BaseCreditMultiplier * npcShip.CargoSpaceTotal;

                // Check if we need to give this trader some credits
                if (npcShip.Credits < baseCreditsSizeAdjusted)
                {
                    // Poor trader has no credits, give him some to start
                    npcShip.Credits = baseCreditsSizeAdjusted;
                }

                // Trader buys first good
                SystemGood good1 = (from g in npcShip.CosmoSystem.SystemGoods
                                    orderby (g.PriceMultiplier) ascending
                                    select g).FirstOrDefault(); 

                // This is the maximum number a Trader can purchase
                double numberCanBuy = Math.Floor((double)npcShip.Credits / good1.Price);

                // This is the maximum number we want the Trader to purchase
                double numberToBuy = Math.Ceiling((double)npcShip.CargoSpaceFree / 2);

                // Make sure that the Trader buys as many of good1 as credits allow
                int numberBuying = (int)Math.Min(numberCanBuy, numberToBuy);
                
                // Insures that Traders attempt to buy the proper number of good1 
                int properNumber = (int)Math.Min(numberBuying, good1.Quantity);
                
                try
                {
                    good1.Buy(npcShip, properNumber, good1.Price);
                }
                catch (InvalidOperationException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "NPC Policy");
                }
               
                // Find all the systems within range
                CosmoSystem[] inRangeSystems = npcShip.GetInRangeSystems();

                // Finds the system with the highest PriceMultiplier 
                CosmoSystem targetSystem = (from g in db.SystemGoods 
                                            where inRangeSystems.Contains(g.CosmoSystem)
                                            && g.Good == good1.Good
                                            orderby (g.PriceMultiplier) descending
                                            select g.CosmoSystem).FirstOrDefault(); 

                // Get references to the Good entities for all the SystemGoods sold in the target system
                IEnumerable<Good> goodsInTargetSystem = targetSystem.SystemGoods.Select(g => g.Good);

                // Get references to the Good entites for the all the SystemGoods sold in the current system
                IEnumerable<Good> goodsInCurrentSystem = npcShip.CosmoSystem.SystemGoods.Select(g => g.Good);
                
                // Do an intersetion of both, getting a list of goods sold in both systems
                IEnumerable<Good> goodsInBoth = goodsInTargetSystem.Intersect(goodsInCurrentSystem);
                
                // Look in the current system for goods sold in both, sorting by PriceMultiplier (lowest at top)
                // and taking the top good in the results
                SystemGood good2 = (from g in npcShip.CosmoSystem.SystemGoods
                                    where goodsInBoth.Contains(g.Good)
                                    && g != good1
                                    orderby g.PriceMultiplier ascending
                                    select g).FirstOrDefault();

                // This is the maximum number a Trader can purchase
                numberCanBuy = Math.Floor((double)npcShip.Credits / good2.Price);

                // This is the maximum number we want the Trader to purchase
                numberToBuy = Math.Ceiling((double)npcShip.CargoSpaceFree);

                // Make sure that the Trader buys as many of good1 as credits allow
                numberBuying = (int)Math.Min(numberCanBuy, numberToBuy);

                // Insures that Traders attempt to buy the proper number of good1 
                properNumber = (int)Math.Min(numberBuying, good2.Quantity);

                try
                {
                    good2.Buy(npcShip, properNumber, good2.Price);
                }
                catch (InvalidOperationException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "NPC Policy");
                }

                this.DoTravel(targetSystem);
                    
                // Set next travel time
                this.npcRow.NextTravelTime = DateTime.UtcNow.AddSeconds(this.rnd.Next(60, 120));
            }
            else
            {
                Dictionary<string, object> props = new Dictionary<string, object>
                {
                    { "NpcId", this.npcRow.NpcId },
                    { "NextTravelTime", this.npcRow.NextTravelTime },
                    { "UtcNow", DateTime.UtcNow }
                };
                Logger.Write("Waiting for NextTravelTime", "NPC", 200, 0, TraceEventType.Verbose, "Trader Wait", props);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Handles Trader combat.
        /// </summary>
        private void DoCombat()
        {
            Ship npcShip = this.npcRow.Ship;
            Combat combat = npcShip.InProgressCombat;
            bool traderTurn = (combat.ShipTurn.ShipId == npcShip.ShipId);
            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "CombatId", combat.CombatId },
                { "TurnShipId", combat.ShipTurn.ShipId },
                { "ShipId", npcShip.ShipId },
                { "TraderTurn", traderTurn }
            };
            Logger.Write("Trader in Combat", "NPC", 150, 0, TraceEventType.Verbose, "Trader in Combat", props);

            if (traderTurn)
            {
                try
                {
                    // We fire our weapon twice
                    for (int i = 0; i < 2; i++)
                    {
                        // Check if our turn is over
                        if (combat.ShipTurn.ShipId != npcShip.ShipId || combat.Status != Combat.CombatStatus.Incomplete)
                        {
                            // Break out
                            break;
                        }

                        // Fire weapon!
                        combat.FireWeapon();

                        Logger.Write("Fired weapon", "NPC", 50, 0, TraceEventType.Verbose, "Trader Combat Turn", props);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Out of turn points
                    ExceptionPolicy.HandleException(ex, "NPC Policy");

                    // Escape
                    combat.ChargeJumpDrive();

                    Logger.Write("Charged JumpDrive", "NPC", 50, 0, TraceEventType.Verbose, "Trader Combat Turn", props);
                }

                // Set a shorter delay before the next action
                this.SetNextActionDelay(NpcTrader.DelayBeforeNextActionCombat);
            }
        }

        /// <summary>
        /// Used when a Trader travels to another system
        /// </summary>
        /// <param name="targetSystem">The target system.</param>
        private void DoTravel(CosmoSystem targetSystem)
        {
            Ship npcShip = this.npcRow.Ship;

            // Start traveling
            int travelTime = npcShip.Travel(targetSystem);

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "TargetSystemId", targetSystem.SystemId },
                { "TravelTime", travelTime },
            };
            Logger.Write("Traveling to new System", "NPC", 150, 0, TraceEventType.Verbose, "Trader Travel", props);
        }
    }
}
