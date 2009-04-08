namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CosmoMonger.Models;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using System.Diagnostics;

    [TestFixture]
    public class CombatBalancingTest : BasePlayerTest
    {
        /// <summary>
        /// Number of mock combats to conduct
        /// </summary>
        public const int TrialCount = 1;

        private Combat combat;
        private Player player1;
        private Player player2;
        private Dictionary<string, List<int>> combatStats = new Dictionary<string, List<int>>();

        [SetUp]
        public void SetupPlayers()
        {
            this.Cleanup();
            player1 = this.CreateTestPlayer();
            player2 = this.CreateTestPlayer();
        }

        private void ResetCombatStats()
        {
            combatStats["Winner"] = new List<int>();
            combatStats["Turns"] = new List<int>();
            combatStats["AttackerHits"] = new List<int>();
            combatStats["AttackerMisses"] = new List<int>();
            combatStats["DefenderHits"] = new List<int>();
            combatStats["DefenderMisses"] = new List<int>();
        }

        private void StartCombat()
        {
            player1.Ship.Attack(player2.Ship);

            combat = player1.Ship.InProgressCombat;
        }

        private void DoCombat()
        {
            // Track number of hits/misses
            int attackerHitCount = 0;
            int attackerMissCount = 0;
            int defenderHitCount = 0;
            int defenderMissCount = 0;
            
            // Track number of turns taken
            int lastTurn = combat.Turn;
            int turnsTaken = 0;

            // Keep going until the combat is over
            while (combat.Status == Combat.CombatStatus.Incomplete)
            {
                try
                {
                    if (combat.Turn == 0)
                    {
                        if (combat.FireWeapon())
                        {
                            attackerHitCount++;
                        }
                        else
                        {
                            attackerMissCount++;
                        }
                    }
                    else
                    {
                        if (combat.FireWeapon())
                        {
                            defenderHitCount++;
                        }
                        else
                        {
                            defenderMissCount++;
                        }
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Not enough turn points to fire weapon, charge jumpdrive instead
                    combat.ChargeJumpDrive();
                }

                if (combat.Turn != lastTurn)
                {
                    // New Turn
                    lastTurn = combat.Turn;
                    turnsTaken++;
                }
            }

            combatStats["Winner"].Add(combat.Turn);
            combatStats["Turns"].Add(turnsTaken);
            combatStats["AttackerHits"].Add(attackerHitCount);
            combatStats["AttackerMisses"].Add(attackerMissCount);
            combatStats["DefenderHits"].Add(defenderHitCount);
            combatStats["DefenderMisses"].Add(defenderMissCount);

            Debug.WriteLine(string.Format("Winner: {0} Turns: {1} Attacker: {2}/{3} Defender: {4}/{5}", combat.Turn, turnsTaken, attackerHitCount, attackerMissCount, defenderHitCount, defenderMissCount));
        }

        private void SetPlayerShip(Player player, string shipName)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Assign the default base ship type
            BaseShip baseShip = (from bs in db.BaseShips
                                 where bs.Name == shipName
                                 select bs).SingleOrDefault();

            player.Ship.BaseShip = baseShip;

            // Setup default upgrades
            player.Ship.JumpDrive = player.Ship.BaseShip.InitialJumpDrive;
            player.Ship.Shield = player.Ship.BaseShip.InitialShield;
            player.Ship.Weapon = player.Ship.BaseShip.InitialWeapon;
        }

        private void SetPlayerRace(Player player, string raceName)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Assign the correct race
            player.Race = (from r in db.Races
                           where r.Name == raceName
                           select r).SingleOrDefault();
        }

        [Test]
        public void HumanTrashCanVsHumanTrashCan()
        {
            // Reset combat stats
            this.ResetCombatStats();

            // Set both players to be human
            this.SetPlayerRace(player1, "Human");
            this.SetPlayerRace(player2, "Human");

            for (int i = 0; i < TrialCount; i++)
            {
                // Make sure each player has the right ship
                this.SetPlayerShip(player1, "Glorified Trash Can");
                this.SetPlayerShip(player2, "Glorified Trash Can");

                this.StartCombat();
                this.DoCombat();
            }

            Debug.WriteLine(string.Format("Average Winner: {0} Turns: {1} Attacker: {2}/{3} Defender: {4}/{5}", combatStats["Winner"].Average(), combatStats["Turns"].Average(), combatStats["AttackerHits"].Average(), combatStats["AttackerMisses"].Average(), combatStats["DefenderHits"].Average(), combatStats["DefenderMisses"].Average()));
        }

        [Test]
        public void HumanTrashCanVsHumanRover()
        {
            // Reset combat stats
            this.ResetCombatStats();

            // Set both players to be human
            this.SetPlayerRace(player1, "Human");
            this.SetPlayerRace(player2, "Human");

            for (int i = 0; i < TrialCount; i++)
            {
                // Make sure each player has the right ship
                this.SetPlayerShip(player1, "Glorified Trash Can");
                this.SetPlayerShip(player2, "Rover");

                this.StartCombat();
                this.DoCombat();
            }

            Debug.WriteLine(string.Format("Average Winner: {0} Turns: {1} Attacker: {2}/{3} Defender: {4}/{5}", combatStats["Winner"].Average(), combatStats["Turns"].Average(), combatStats["AttackerHits"].Average(), combatStats["AttackerMisses"].Average(), combatStats["DefenderHits"].Average(), combatStats["DefenderMisses"].Average()));
        }

        [Test]
        public void HumanRoverVsHumanRover()
        {
            // Reset combat stats
            this.ResetCombatStats();

            // Set both players to be human
            this.SetPlayerRace(player1, "Human");
            this.SetPlayerRace(player2, "Human");

            for (int i = 0; i < TrialCount; i++)
            {
                // Make sure each player has the right ship
                this.SetPlayerShip(player1, "Rover");
                this.SetPlayerShip(player2, "Rover");

                this.StartCombat();
                this.DoCombat();
            }

            Debug.WriteLine(string.Format("Average Winner: {0} Turns: {1} Attacker: {2}/{3} Defender: {4}/{5}", combatStats["Winner"].Average(), combatStats["Turns"].Average(), combatStats["AttackerHits"].Average(), combatStats["AttackerMisses"].Average(), combatStats["DefenderHits"].Average(), combatStats["DefenderMisses"].Average()));
        }
    }
}
