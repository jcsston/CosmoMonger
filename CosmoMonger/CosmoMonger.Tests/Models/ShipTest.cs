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
    }
}
