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

            // Randomly select an aggression level between -10 and 10
            // -10 is a nice pirate, 10 is a bloodthirsty pirate
            this.npcRow.Aggression = this.rnd.Next(-10, 11);
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        public override void DoAction()
        {
            if (!this.SetNextActionDelay(NpcShipBase.DelayBeforeNextAction))
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
                if (npcShip.Credits < BaseCreditAmount)
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

                this.SetNextTravelTime();
            } 
            else 
            {
                /*
                Dictionary<string, object> props = new Dictionary<string, object>
                {
                    { "NpcId", this.npcRow.NpcId },
                    { "NextTravelTime", this.npcRow.NextTravelTime },
                    { "UtcNow", DateTime.UtcNow }
                };
                Logger.Write("Waiting for NextTravelTime", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Wait", props);
                */
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
            bool pirateTurn = (combat.ShipTurn.ShipId == npcShip.ShipId);
            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "NpcId", this.npcRow.NpcId },
                { "CombatId", combat.CombatId },
                { "TurnShipId", combat.ShipTurn.ShipId },
                { "ShipId", npcShip.ShipId },
                { "PirateTurn", pirateTurn }
            };
            Logger.Write("Pirate in Combat", "NPC", 150, 0, TraceEventType.Verbose, "Pirate in Combat", props);

            if (pirateTurn)
            {
                try
                {
                    // Has the other ship offered a surrender?
                    if (combat.Surrendered)
                    {
                        // Do we accept?

                        // Add the aggression to a random number, if non-negative we accept surrender
                        int netResult = this.rnd.Next(-5, 10) + this.npcRow.Aggression;
                        if (netResult >= 0)
                        {
                            // Accept surrender
                            combat.AcceptSurrender();
                            return;
                        }
                    }
                    else if (combat.CargoJettisoned)
                    {
                        // Do we pickup the jettisoned cargo?
                        
                        // We only even look at the cargo if there are more than 6-12 items
                        int cargoCount = combat.CargoJettisonedCount;
                        int minPickupAmount = this.rnd.Next(6, 12);
                        if (cargoCount >= minPickupAmount)
                        {
                            // Check if we have space for at least 50% of the of the items
                            if (npcShip.CargoSpaceFree >= cargoCount / 2)
                            {
                                // Pickup the cargo
                                combat.PickupCargo();
                                return;
                            }
                        }
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

                        Logger.Write("Fired weapon", "NPC", 50, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Out of turn points
                    ExceptionPolicy.HandleException(ex, "NPC Policy");

                    // Escape
                    combat.ChargeJumpDrive();

                    Logger.Write("Charged JumpDrive", "NPC", 50, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);
                }

                // Set a shorter delay before the next action
                this.SetNextActionDelay(NpcShipBase.DelayBeforeNextActionCombat);
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

            // Exclude ships that have more than 3 seconds left to jum,
            // police ships, and the last ship we attacked
            IEnumerable<Ship> targetableShips = (from s in attackableShips
                                                 where s.TargetSystemArrivalTime.Value.AddSeconds(3) > DateTime.UtcNow
                                                 && (s.Npcs.Any(n => n.NType != Npc.NpcType.Police)
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
            Logger.Write("Looking for ships to attack", "NPC", 100, 0, TraceEventType.Verbose, "Pirate Prowl", props);

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
            Logger.Write("Traveling to new System", "NPC", 150, 0, TraceEventType.Verbose, "Pirate Travel", props);
        }
    }
}
