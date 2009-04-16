//-----------------------------------------------------------------------
// <copyright file="NpcShipBase.cs" company="CosmoMonger">
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
    /// Base class for NPCs with ships
    /// </summary>
    public abstract class NpcShipBase : NpcBase
    {
        /// <summary>
        /// Standard delay min to wait between traveling
        /// </summary>
        public const int DelayBeforeNextTravelMin = 45;

        /// <summary>
        /// Standard delay max to wait between traveling
        /// </summary>
        public const int DelayBeforeNextTravelMax = 90;

        /// <summary>
        /// Standard delay to wait between actions
        /// </summary>
        public static TimeSpan DelayBeforeNextAction = new TimeSpan(0, 0, 0, 4);

        /// <summary>
        /// Combat delay to wait between actions
        /// </summary>
        public static TimeSpan DelayBeforeNextActionCombat = new TimeSpan(0, 0, 0, 0, 750);

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcShipBase"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcShipBase(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Create the ship for the NPC
        /// </summary>
        public override void Setup()
        {
            base.Setup();

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Select a random race
            Race npcRace = this.rnd.SelectOne(db.Races);
            
            // Assign the race
            this.npcRow.Race = npcRace;

            // We only start in systems with the same race
            IQueryable<CosmoSystem> possibleStartingSystems = (from s in db.CosmoSystems
                                                               where s.Race == npcRace
                                                               select s);
            CosmoSystem startingSystem = this.rnd.SelectOne(possibleStartingSystems);

            // Randomly select a base ship model
            BaseShip startingShipModel = this.rnd.SelectOne(db.BaseShips);

            // Create the NPC ship
            Ship npcShip = startingSystem.CreateShip(startingShipModel.Name);
            this.npcRow.Ship = npcShip;

            // Randomly assign upgrades
            npcShip.JumpDrive = this.rnd.SelectOne(db.JumpDrives);
            npcShip.Shield = this.rnd.SelectOne(db.Shields);
            npcShip.Weapon = this.rnd.SelectOne(db.Weapons);

            // Set the next travel time to now
            this.npcRow.NextTravelTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Sets the next travel time, random number of seconds between NpcShipBase.DelayBeforeNextTravelMin
        /// and NpcShipBase.DelayBeforeNextTravelMax in the future.
        /// </summary>
        public void SetNextTravelTime()
        {
            int delayBeforeNextTravel = this.rnd.Next(NpcShipBase.DelayBeforeNextTravelMin, NpcShipBase.DelayBeforeNextTravelMax);
            this.npcRow.NextTravelTime = DateTime.UtcNow.AddSeconds(delayBeforeNextTravel);
        }
    }
}
