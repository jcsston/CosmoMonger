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

    /// <summary>
    /// Summary description for ShipTest
    /// </summary>
    [TestFixture]
    public class ShipTest : BasePlayerTest
    {
        [Test]
        public void GetInRangeSystems()
        {
            Player testPlayer = this.CreateTestPlayer();

            CosmoSystem [] inRangeSystems =  testPlayer.Ship.GetInRangeSystems();

            Assert.That(inRangeSystems, Is.Not.CollectionContaining(testPlayer.Ship.CosmoSystem), "In range systems should not include current system");
        }

        [Test]
        public void CargoSpaceFree()
        {
            // Arrange
            Ship ship = new Ship();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.CargoSpace = 30;

            ship.JumpDrive = new JumpDrive();
            ship.JumpDrive.CargoCost = 1;

            ship.Shield = new Shield();
            ship.Shield.CargoCost = 2;

            ship.Weapon = new Weapon();
            ship.Weapon.CargoCost = 3;

            // Assert
            Assert.That(ship.CargoSpaceFree, Is.EqualTo(30-6), "Total free cargo space should equal the BaseShip amount minus 6");
        }

        [Test]
        public void CargoSpaceTotal()
        {
            // Arrange
            BaseShip baseShip = new BaseShip();
            baseShip.CargoSpace = 30;
            
            Ship ship = new Ship();
            ship.BaseShip = baseShip;

            // Assert
            Assert.That(ship.CargoSpaceTotal, Is.EqualTo(30), "Total cargo space should equal the BaseShip amount");
        }

        [Test]
        public void TradeInValue()
        {
            // Arrange
            Ship ship = new Ship();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.BasePrice = 1000;
            ship.CosmoSystem = new CosmoSystem();

            // Assert
            Assert.That(ship.TradeInValue, Is.EqualTo(800), "Trade in value should equal 80% of the BaseShip value amount");
        }

        [Test]
        public void TradeInValueFromSystem()
        {
            // Arrange
            Ship ship = new Ship();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.BasePrice = 1000;
            ship.CosmoSystem = new CosmoSystem();
            SystemShip systemShip = new SystemShip();
            systemShip.BaseShip = ship.BaseShip;
            systemShip.PriceMultiplier = 2.0;
            ship.CosmoSystem.SystemShips.Add(systemShip);

            // Assert
            Assert.That(ship.TradeInValue, Is.EqualTo(1600), "Trade in value should equal 80% of 2x BaseShip value amount");
        }

        private int player1Id;
        private int player2Id;
        private int player1AttackCount;
        private int player2AttackCount;

        public void AttackAtSameTimeThread()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Player player1 = db.Players.Where(p => p.PlayerId == player1Id).Single();
            Player player2 = db.Players.Where(p => p.PlayerId == player2Id).Single();

            for (int i = 0; i < 100; i++)
            {
                try
                {
                    player1.Ship.Attack(player2.Ship);
                    player1.Ship.InProgressCombat.End();
                    player1AttackCount++;
                }
                catch (ArgumentException ex)
                {

                }
                catch (InvalidOperationException ex)
                {

                }
            }
        }

        [Test]
        public void AttackAtSameTime()
        {
            Player player1 = this.CreateTestPlayer();
            Player player2 = this.CreateTestPlayer();
            player1Id = player1.PlayerId;
            player2Id = player2.PlayerId;
            player1AttackCount = 0;
            player2AttackCount = 0;

            Thread t = new Thread(new ThreadStart(this.AttackAtSameTimeThread));
            t.Start();
            
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    player2.Ship.Attack(player1.Ship);
                    player2.Ship.InProgressCombat.End();
                    player2AttackCount++;
                }
                catch (ArgumentException ex)
                {
                    // Good
                }
                catch (InvalidOperationException ex)
                {
                    // Good
                }
            }

            t.Join();
            Console.WriteLine("Attacked Player 1: {0} Player 2: {1}", player1AttackCount, player2AttackCount);
        }

        [Test]
        public void Attack()
        {
            Player player1 = this.CreateTestPlayer();
            Player player2 = this.CreateTestPlayer();

            player1.Ship.Attack(player2.Ship);
            while (player2.Ship.DamageHull < 100)
            {
                Console.WriteLine("Player 2 Shield {0} Hull: {1}", player2.Ship.DamageShield, player2.Ship.DamageHull);
                player1.Ship.InProgressCombat.FireWeapon();
                // Max out turn points for testing
                player1.Ship.InProgressCombat.TurnPointsLeft = 999;
            }
            player1.Ship.InProgressCombat.End();
        }
    }
}
