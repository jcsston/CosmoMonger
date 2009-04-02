//-----------------------------------------------------------------------
// <copyright file="NpcCleaner.cs" company="CosmoMonger">
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
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This NPC cleans the galaxy of stalled players. Keeping the galaxy clean and in good working order.
    /// </summary>
    public class NpcCleaner : NpcBase
    {
        /// <summary>
        /// Constant for the number of minutes to schedule between cleaner sweeps.
        /// </summary>
        public const int MinutesBetweenCleanerSweeps = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcCleaner"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcCleaner(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Cleaner NPC. Looks for inactive users and makes sure that are not stuck in travel.
        /// </summary>
        public override void DoAction()
        {
            Logger.Write("Enter NpcCleaner.DoAction", "NPC", 100, 0, TraceEventType.Verbose);
            
            // Mark the next time traveling players will need cleaning
            if (!this.SetNextActionDelay(new TimeSpan(0, NpcCleaner.MinutesBetweenCleanerSweeps, 0)))
            {
                // Another thread has made changes to this Npc, should exit
                return;
            }

            // Find traveling ships past due & players who haven't been active for the past minute
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            var shipsNeedingCleaning = (from p in db.Players
                                        where p.Ship.TargetSystemArrivalTime < DateTime.UtcNow
                                        && p.LastPlayed.AddMinutes(1) < DateTime.UtcNow
                                        select p.Ship);
            foreach (Ship ship in shipsNeedingCleaning)
            {
                // Check that the ship is not in combat
                if (ship.InProgressCombat == null)
                {
                    // Fix travel state
                    ship.CheckIfTraveling();
                }
            }
        }
    }
}
