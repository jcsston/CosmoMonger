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
            if (!this.SetNextActionDelay(new TimeSpan(0, 0, 10)))
            {
                return;
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            Ship npcShip = this.NpcRow.Ship;

            // Check if we are still traveling
            npcShip.CheckIfTraveling();

            Dictionary<string, object> props;

            // Check if we are currently in combat
            if (npcShip.InProgressCombat != null)
            {
                // In Combat!
                Combat combat = npcShip.InProgressCombat;

                props = new Dictionary<string, object>
                {
                    { "NpcId", this.NpcRow.NpcId },
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
                        while (combat.ShipTurn.ShipId == npcShip.ShipId)
                        {
                            // Fire until we cannot any more!
                            combat.FireWeapon();

                            Logger.Write("Fired weapon", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Combat Turn", props);

                            // Wait five seconds before firing again
                            //Thread.Sleep(5000);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Combat over, ignore
                        ExceptionPolicy.HandleException(ex, "NPC Policy");
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
                }
            }
            else if (this.NpcRow.NextTravelTime < DateTime.UtcNow)
            {
                // Attack?
                if (this.rnd.SelectByProbablity(new bool[] { true, false }, new double[] { 0.50, 0.50 }))
                {
                    // Look for a ship to attack
                    IEnumerable<Ship> attackableShips = npcShip.GetShipsToAttack();

                    // Exclude police ships
                    IEnumerable<Ship> targetableShips = (from s in attackableShips
                                                         where s.Npcs.Any(n => n.NType != NpcType.Police)
                                                         || s.Players.Any()
                                                         select s).AsEnumerable();

                    props = new Dictionary<string, object>
                    {
                        { "NpcId", this.NpcRow.NpcId },
                        { "AttackableShips", attackableShips.Count() },
                        { "TargetableShips", targetableShips.Count() },
                    };
                    Logger.Write("Looking for ships to attack", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Prowl", props);

                    Ship shipToAttack = this.rnd.SelectOne(targetableShips);
                    if (shipToAttack != null)
                    {
                        props = new Dictionary<string, object>
                        {
                            { "NpcId", this.NpcRow.NpcId },
                            { "TargetShipId", shipToAttack.ShipId }
                        };
                        Logger.Write("Attacking Ship", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Attacking", props);

                        try
                        {
                            // Attack!
                            npcShip.Attack(shipToAttack);
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
                else
                {
                    // We travel to any system in range
                    CosmoSystem[] inRangeSystems = npcShip.GetInRangeSystems();
                    CosmoSystem targetSystem = this.rnd.SelectOne(inRangeSystems);

                    // Start traveling
                    int travelTime = npcShip.Travel(targetSystem);

                    props = new Dictionary<string, object>
                    {
                        { "NpcId", this.NpcRow.NpcId },
                        { "TargetSystemId", targetSystem.SystemId },
                        { "TravelTime", travelTime },
                    };
                    Logger.Write("Traveling to new System", "NPC", 200, 0, TraceEventType.Verbose, "Pirate Travel", props);
                }

                // Set next travel time
                this.NpcRow.NextTravelTime = DateTime.UtcNow.AddMinutes(1);
            }

            try
            {
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to one of this npc record
                // Overwrite those changes
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }
        }
    }
}
