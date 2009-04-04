//-----------------------------------------------------------------------
// <copyright file="NpcPirate.cs" company="CosmoMonger">
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
    using System.Threading;
    using System.Web;
    using CosmoMonger.Models.Utility;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Contains logic for NPC Pirates
    /// </summary>
    public class NpcPirate : NpcShipBase
    {
        /// <summary>
        /// The base of amount of credits we always ensure that a pirate has aboard
        /// </summary>
        public const int BaseCreditAmount = 1000;

        /// <summary>
        /// Standard delay to wait between traveling and attacking
        /// </summary>
        public static TimeSpan DelayBeforeNextTravel = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Standard delay to wait between actions
        /// </summary>
        public static TimeSpan DelayBeforeNextAction = new TimeSpan(0, 0, 0, 5);

        /// <summary>
        /// Combat delay to wait between actions
        /// </summary>
        public static TimeSpan DelayBeforeNextActionCombat = new TimeSpan(0, 0, 0, 0, 750);

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcPirate"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcPirate(Npc npcRow)
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
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        public override void DoAction()
        {
            if (!this.SetNextActionDelay(NpcPirate.DelayBeforeNextAction))
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
                // Check if we need to give this pirate some credits
                if (npcShip.Credits < NpcPirate.BaseCreditAmount)
                {
                    // Poor pirate has no gold, give him some to start
                    npcShip.Credits = BaseCreditAmount;
                }

                // Attack?
                if (this.rnd.SelectByProbablity(new bool[] { true, false }, new double[] { 0.50, 0.50 }))
                {
                    this.DoAttack();
                }
                else
                {
                    this.DoTravel();
                }

                // Set next travel time
                this.npcRow.NextTravelTime = DateTime.UtcNow.Add(NpcPirate.DelayBeforeNextTravel);
            } 
            else 
            {
                Dictionary<string, object> props = new Dictionary<string, object>
                {
                    { "NpcId", this.npcRow.NpcId },
                    { "NextTravelTime", this.npcRow.NextTravelTime },
                    { "UtcNow", DateTime.UtcNow }
                };
                Logger.Write("Waiting for NextTravelTime", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Wait", props);
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Handles Pirate Combat
        /// </summary>
        private void DoCombat()
        {
            Ship npcShip = this.npcRow.Ship;
            Combat combat = npcShip.InProgressCombat;

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "CombatId", combat.CombatId },
                { "TurnShipId", combat.ShipTurn.ShipId },
                { "ShipId", npcShip.ShipId }
            };
            Logger.Write("Pirate in Combat", "NPC", 150, 0, TraceEventType.Verbose, "Pirate in Combat", props);

            if (combat.ShipTurn.ShipId == npcShip.ShipId)
            {
                Logger.Write("Pirate Combat Turn", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);

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

                        Logger.Write("Fired weapon", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Out of turn points
                    ExceptionPolicy.HandleException(ex, "NPC Policy");

                    // Escape
                    combat.ChargeJumpDrive();

                    Logger.Write("Charged JumpDrive", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);
                }

                Logger.Write("Ended Combat Turn", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);

                // Set a shorter delay before the next action
                this.SetNextActionDelay(NpcPirate.DelayBeforeNextActionCombat);
            }
        }

        /// <summary>
        /// Handles a Pirate selecting and attacking a ship
        /// </summary>
        private void DoAttack()
        {
            Ship npcShip = this.npcRow.Ship;

            // Look for a ship to attack
            IEnumerable<Ship> attackableShips = npcShip.GetShipsToAttack();

            // Exclude police ships
            IEnumerable<Ship> targetableShips = (from s in attackableShips
                                                 where (s.Npcs.Any(n => n.NType != NpcType.Police)
                                                 || s.Players.Any())
                                                 && s != this.npcRow.LastAttackedShip
                                                 select s).AsEnumerable();

            // Filter out ships that are already in combat
            targetableShips = (from s in targetableShips
                               where s.InProgressCombat == null
                               select s).AsEnumerable();

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "AttackableShips", attackableShips.Count() },
                { "TargetableShips", targetableShips.Count() },
            };
            Logger.Write("Looking for ships to attack", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Prowl", props);

            Ship shipToAttack = this.rnd.SelectOne(targetableShips);
            if (shipToAttack != null)
            {
                props = new Dictionary<string, object>
                {
                    { "NpcId", this.npcRow.NpcId },
                    { "TargetShipId", shipToAttack.ShipId }
                };
                Logger.Write("Attacking Ship", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Attacking", props);

                try
                {
                    // Attack!
                    npcShip.Attack(shipToAttack);
                    this.npcRow.LastAttackedShip = shipToAttack;
                    Logger.Write("Attacked Ship", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Attacked", props);
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
        /// Handles the pirate traveling to a new system
        /// </summary>
        private void DoTravel()
        {
            Ship npcShip = this.npcRow.Ship;

            // We travel to any system in range
            CosmoSystem[] inRangeSystems = npcShip.GetInRangeSystems();
            CosmoSystem targetSystem = this.rnd.SelectOne(inRangeSystems);

            // Start traveling
            int travelTime = npcShip.Travel(targetSystem);

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "TargetSystemId", targetSystem.SystemId },
                { "TravelTime", travelTime },
            };
            Logger.Write("Traveling to new System", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Travel", props);
        }
    }
}
