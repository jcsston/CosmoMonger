//-----------------------------------------------------------------------
// <copyright file="NpcBalancer.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using CosmoMonger.Models.Utility;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This class balances the number of active NPCs vs active Players to keep the galaxy alive.
    /// </summary>
    public class NpcBalancer : NpcBase
    {
        /// <summary>
        /// The number of players we want active at all time
        /// </summary>
        public const int TargetActivePlayers = 25;

        /// <summary>
        /// The mininum number of Npcs that should always be active
        /// </summary>
        public const int MinNpcs = 10;

        /// <summary>
        /// Percent of generated traders
        /// </summary>
        public const double PercentTraders = 0.50;

        /// <summary>
        /// Percent of generated pirates
        /// </summary>
        public const double PercentPirates = 0.25;

        /// <summary>
        /// Percent of generated police
        /// </summary>
        public const double PercentPolice = 0.25;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBalancer"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcBalancer(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Balances the number of NPCs vs Active Players to keep the galaxy alive.
        /// </summary>
        public override void DoAction()
        {
            // Set next run time to 1 hour
            if (!this.SetNextActionDelay(new TimeSpan(1, 0, 0))) 
            {
                return;
            }

            // Balance Npcs
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Find the number of players active in the last hour on the galaxy
            int activePlayerCount = (from p in db.Players
                                     where p.LastPlayed > DateTime.UtcNow.AddHours(-1)
                                     select p).Count();
            
            // Calculate how many Npcs we need
            int neededNpcs = NpcBalancer.TargetActivePlayers - activePlayerCount;

            // Make sure we don't go below the min amount
            neededNpcs = Math.Max(neededNpcs, NpcBalancer.MinNpcs);

            // Grab the active NPCs we currently have
            IQueryable<Npc> activeNpcs = (from n in db.Npcs
                                          where n.NType == Npc.NpcType.Pirate
                                          || n.NType == Npc.NpcType.Police
                                          || n.NType == Npc.NpcType.Trader
                                          select n);
            
            // Calculate how many NPCs we need to create/delete
            int npcAdjustment = neededNpcs - activeNpcs.Count();

            if (npcAdjustment > 0)
            {
                while (npcAdjustment-- > 0)
                {
                    // Produce NPCs
                    Npc newNpc = new Npc();
                    NpcBase npc = null;

                    // Which type will we produce?
                    //NpcType[] npcTypes = { NpcType.Pirate, NpcType.Trader, NpcType.Police };
                    // Only Pirates for now
                    Npc.NpcType[] npcTypes = { Npc.NpcType.Pirate, Npc.NpcType.Police, Npc.NpcType.Police };
                    double[] npcProbablity = { NpcBalancer.PercentPirates, NpcBalancer.PercentTraders, NpcBalancer.PercentPolice };
                    newNpc.NType = this.rnd.SelectByProbablity(npcTypes, npcProbablity);

                    switch (newNpc.NType)
                    {
                        case Npc.NpcType.Pirate:
                            // Produce pirate
                            npc = new NpcPirate(newNpc);
                            break;

                        case Npc.NpcType.Trader:
                            // Produce trader
                            npc = new NpcTrader(newNpc);
                            break;

                        case Npc.NpcType.Police:
                            // Produce Police
                            npc = new NpcPolice(newNpc);
                            break;

                        default:
                            throw new InvalidOperationException("Unknown NpcType selected");
                    }

                    // Give the NPC a good name
                    string npcName = (from name in db.NpcNames
                                      where name.NType == newNpc.NType
                                      && !(from n in db.Npcs
                                           select n.Name)
                                          .Contains(name.Name)
                                      select name.Name).FirstOrDefault();

                    // Break out if we couldn't find a new name
                    if (npcName == null)
                    {
                        Logger.Write(newNpc.NType.ToString(), "NPC", 1000, 0, TraceEventType.Critical, "Out of NPC Names");
                        break;
                    }

                    // Give the NPC the cool name
                    newNpc.Name = npcName;

                    // Add to the database
                    db.Npcs.InsertOnSubmit(newNpc);

                    // Setup the NPC
                    npc.Setup();

                    db.SaveChanges();
                }
            }
            else if (npcAdjustment < 0)
            {
                // TODO: Delete or inactive NPCs
                /*
                // Query the NPCs to delete
                IQueryable<Npc> npcsToDelete = activeNpcs.Take(Math.Abs(npcAdjustment));
                if (npcsToDelete.Any())
                {
                    // Remove the NPCs
                    db.Npcs.DeleteAllOnSubmit(npcsToDelete);

                    IQueryable<Ship> npcShips = (from n in activeNpcs
                                                 select n.Ship);
                    if (npcShips.Any())
                    {
                        IQueryable<ShipGood> npcShipGoods = (from g in db.ShipGoods
                                                             where npcShips.Contains(g.Ship)
                                                             select g);
                        if (npcShipGoods.Any())
                        {
                            // Remove the ship goods
                            db.ShipGoods.DeleteAllOnSubmit(npcShipGoods);
                        }

                        // Remove the ships
                        db.Ships.DeleteAllOnSubmit(npcShips);
                    }
                }
                */
            }

            db.SaveChanges();
        }
    }
}
