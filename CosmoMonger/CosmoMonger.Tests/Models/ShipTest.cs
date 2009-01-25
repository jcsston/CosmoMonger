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
        private Random rnd = new Random();
        private int shipTravelLoopCount = 5;
        private int shipTravelThreadCount = 10;

        [Test]
        [CategoryAttribute("LongRunning")]
        public void Travel()
        {
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            
            CosmoSystem [] possibleTargetSystems = testShip.GetInRangeSystems();
            Assert.AreNotEqual(0, possibleTargetSystems.Length, "Ship should always be within range of at least one system");

            CosmoSystem targetSystem = possibleTargetSystems[0];

            DateTime startTime = DateTime.Now;
            int timeToTravel = testShip.Travel(targetSystem);
            DateTime endTime = DateTime.Now;
            DateTime travelTime = startTime.AddSeconds(timeToTravel);

            while (testShip.CheckIfTraveling())
            {
                endTime = DateTime.Now;
                Thread.Sleep(0);
            }

            // Calcuate the difference between predicted travel time and actual travel time
            TimeSpan travelTimeDiff = endTime - travelTime;
            Assert.AreEqual(0.0, travelTimeDiff.TotalSeconds, 2.0, "Ship have should taken " + timeToTravel + " seconds to travel, +/-2.0 seconds.");

            Assert.AreEqual(targetSystem, testShip.CosmoSystem, "Ship should now be in the target system");
        }

        [Test]
        [CategoryAttribute("LongRunning")]
        public void TravelRandom()
        {
            TravelRandom(0);
        }

        [Test]
        [CategoryAttribute("LongRunning")]
        public void TravelRandomThreaded()
        {
            Thread[] t = new Thread[this.shipTravelThreadCount];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Thread(new ParameterizedThreadStart(this.TravelRandom));
                t[i].Start(i);
            }
            for (int i = 0; i < t.Length; i++)
            {
                t[i].Join();
            }
        }

        private void TravelRandom(object playerIndex)
        {
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;

            for (int i = 0; i < this.shipTravelLoopCount; i++)
            {
                CosmoSystem[] possibleTargetSystems = testShip.GetInRangeSystems();
                Assert.AreNotEqual(0, possibleTargetSystems.Length, "Ship should always be within range of at least one system");

                CosmoSystem targetSystem = possibleTargetSystems[this.rnd.Next(possibleTargetSystems.Length)];

                DateTime startTime = DateTime.Now;
                int timeToTravel = testShip.Travel(targetSystem);
                DateTime endTime = DateTime.Now;
                DateTime travelTime = startTime.AddSeconds(timeToTravel);

                while (testShip.CheckIfTraveling())
                {
                    endTime = DateTime.Now;
                    Thread.Sleep(0);
                }

                // Calcuate the difference between predicted travel time and actual travel time
                TimeSpan travelTimeDiff = endTime - travelTime;
                Assert.AreEqual(0.0, travelTimeDiff.TotalSeconds, 2.0, "Ship have should taken " + timeToTravel + " seconds to travel, +/-2.0 seconds.");

                Assert.AreEqual(targetSystem, testShip.CosmoSystem, "Ship should now be in the target system");
            }
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
    }
}
