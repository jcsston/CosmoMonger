//-----------------------------------------------------------------------
// <copyright file="NpcPolice.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using System.Diagnostics;

    /// <summary>
    /// Contains logic for NPC Police
    /// </summary>
    public class NpcPolice : NpcShipBase
    {
        /// <summary>
        /// Standard delay to wait between traveling and attacking
        /// </summary>
        public static TimeSpan DelayBeforeNextTravel = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Standard delay to wait between actions
        /// </summary>
        public static TimeSpan DelayBeforeNextAction = new TimeSpan(0, 0, 0, 4);

        /// <summary>
        /// Combat delay to wait between actions
        /// </summary>
        public static TimeSpan DelayBeforeNextActionCombat = new TimeSpan(0, 0, 0, 0, 750);

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcPolice"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcPolice(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Setup the new Npc in the database, not required for all types.
        /// If the Npc has a ship this should be overrided to handle creation of the ship.
        /// </summary>
        public override void Setup()
        {
            // Call base class setup method
            // After this is where you would place more setup code
            base.Setup();

            // Randomly select an aggression level between -10 and 10
            // -10 is a nice officer, 10 is a thug
            this.npcRow.Aggression = this.rnd.Next(-10, 11);
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        public override void DoAction()
        {
            if (!this.SetNextActionDelay(DelayBeforeNextAction))
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
                // Search?
                if (this.rnd.SelectByProbablity(new bool[] { true, false }, new double[] { 0.50, 0.50 }))
                {
                    this.DoSearch();
                }
                else
                {
                    this.DoTravel();
                }

                // Set next travel time
                this.npcRow.NextTravelTime = DateTime.UtcNow.Add(DelayBeforeNextTravel);
            }
            else
            {
                Dictionary<string, object> props = new Dictionary<string, object>
                {
                    { "NpcId", this.npcRow.NpcId },
                    { "NextTravelTime", this.npcRow.NextTravelTime },
                    { "UtcNow", DateTime.UtcNow }
                };
                Logger.Write("Waiting for NextTravelTime", "NPC", 200, 0, TraceEventType.Verbose, "Police Wait", props);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Handles Police Combat
        /// </summary>
        public void DoCombat()
        {
            Ship npcShip = this.npcRow.Ship;
            Combat combat = npcShip.InProgressCombat;
            bool policeTurn = (combat.ShipTurn.ShipId == npcShip.ShipId);
            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "CombatId", combat.CombatId },
                { "TurnShipId", combat.ShipTurn.ShipId },
                { "ShipId", npcShip.ShipId },
                { "PoliceTurn", policeTurn }
            };
            Logger.Write("Police in Combat", "NPC", 150, 0, TraceEventType.Verbose, "Police in Combat", props);

            if (policeTurn)
            {
                try
                {
                    // Has the other ship offered a surrender?
                    if (combat.Surrendered)
                    {
                        // Do we accept?
                        // Accept surrender
                        combat.AcceptSurrender();
                    }
                    else if (combat.CargoJettisoned)
                    {
                        // Pickup the cargo
                        combat.PickupCargo();
                    }

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

                        Logger.Write("Fired weapon", "NPC", 50, 0, TraceEventType.Verbose, "Police Combat Turn", props);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Out of turn points
                    ExceptionPolicy.HandleException(ex, "NPC Policy");

                    // Escape
                    combat.ChargeJumpDrive();

                    Logger.Write("Charged JumpDrive", "NPC", 50, 0, TraceEventType.Verbose, "Police Combat Turn", props);
                }

                // Set a shorter delay before the next action
                this.SetNextActionDelay(DelayBeforeNextActionCombat);
            }
        }

        /// <summary>
        /// Handles a Police selecting and searching a ship
        /// </summary>
        public void DoSearch()
        {
            Ship npcShip = this.npcRow.Ship;

            // Look for a ship to search
            IEnumerable<Ship> searchableShips = npcShip.GetShipsToAttack();

            // Only look at player ships, excluding the last ship we searched/attacked
            IEnumerable<Ship> targetableShips = (from s in searchableShips
                                                 where s.Players.Any()
                                                 && s.ShipId != this.npcRow.LastAttackedShipId
                                                 select s).AsEnumerable();

            // Filter out ships that are already in combat
            targetableShips = (from s in targetableShips
                               where s.InProgressCombat == null
                               select s).AsEnumerable();

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "AttackableShips", searchableShips.Count() },
                { "TargetableShips", targetableShips.Count() },
            };
            Logger.Write("Looking for ships to search", "NPC", 100, 0, TraceEventType.Verbose, "Police Patrol", props);

            Ship shipToSearch = this.rnd.SelectOne(targetableShips);
            if (shipToSearch != null)
            {
                props = new Dictionary<string, object>
                {
                    { "NpcId", this.npcRow.NpcId },
                    { "TargetShipId", shipToSearch.ShipId }
                };
                Logger.Write("Searching Ship", "NPC", 200, 0, TraceEventType.Verbose, "Police Searching", props);

                try
                {
                    // Search !
                    npcShip.Attack(shipToSearch);
                    
                    // Start the search
                    npcShip.InProgressCombat.StartSearch();

                    this.npcRow.LastAttackedShip = shipToSearch;
                    Logger.Write("Attacked Ship", "NPC", 150, 0, TraceEventType.Verbose, "Police Searched", props);
                }
                catch (InvalidOperationException ex)
                {
                    ExceptionPolicy.HandleException(ex, "NPC Policy");
                }
                catch (ArgumentException ex)
                {
                    ExceptionPolicy.HandleException(ex, "NPC Policy");
                }
            }
        }

        /// <summary>
        /// Handles the police traveling to a new system
        /// </summary>
        public void DoTravel()
        {
            Ship npcShip = this.npcRow.Ship;

            // We travel to any system of the same race in range
            CosmoSystem[] inRangeSystems = npcShip.GetInRangeSystems();

            // Filter to only include systems of the same race
            IEnumerable<CosmoSystem> targetSystems = (from s in inRangeSystems
                                                      where s.Race == this.npcRow.Race
                                                      select s).AsEnumerable();
            CosmoSystem targetSystem = this.rnd.SelectOne(targetSystems);

            // Start traveling
            int travelTime = npcShip.Travel(targetSystem);

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "TargetSystemId", targetSystem.SystemId },
                { "TravelTime", travelTime },
            };
            Logger.Write("Traveling to new System", "NPC", 150, 0, TraceEventType.Verbose, "Police Travel", props);
        }
    }
}
