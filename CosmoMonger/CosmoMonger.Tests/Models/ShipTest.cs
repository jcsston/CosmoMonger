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

    /// <summary>
    /// Summary description for ShipTest
    /// </summary>
    [TestFixture]
    public class ShipTest
    {
        Random rnd = new Random();

        public ShipTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [Test]
        public void ShipTravel()
        {
            Player testPlayer = PlayerTest.CreateTestPlayer();
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
        public void ShipTravelRandom()
        {
            Player testPlayer = PlayerTest.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;

            for (int i = 0; i < 5; i++)
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
        public void ShipTravelRandomThreaded()
        {
            Thread[] t = new Thread[10];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Thread(new ThreadStart(this.ShipTravelRandom));
                t[i].Start();
            }
            for (int i = 0; i < t.Length; i++)
            {
                t[i].Join();
            }
        }
    }
}
