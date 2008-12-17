namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Linq;
    using CosmoMonger.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for ShipTest
    /// </summary>
    [TestClass]
    public class ShipTest
    {
        Random rnd = new Random();

        public ShipTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private Player CreateTestPlayer()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            UserManager userManager = new UserManager();
            
            string postFix = DateTime.Now.ToBinary().ToString();
            string baseTestUsername = "testUser_" + postFix;
            string baseTestEmail = "testUser_" + postFix + "@cosmomonger.com";
            User testUser = userManager.CreateUser("ship" + baseTestUsername, "test1000", "ship" + baseTestEmail);

            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            return testUser.CreatePlayer("ship" + baseTestUsername, humanRace);
        }

        [TestMethod]
        public void TestTravel()
        {
            Player testPlayer = CreateTestPlayer();
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

        [TestMethod]
        public void TestTravelThreaded()
        {
            Thread[] t = new Thread[5];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Thread(new ThreadStart(this.TestTravel));
                t[i].Start();
            }
            for (int i = 0; i < t.Length; i++)
            {
                t[i].Join();
            }
        }

        [TestMethod]
        public void TestTravelRandom()
        {
            Player testPlayer = CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;

            for (int i = 0; i < 10; i++)
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

        [TestMethod]
        public void TestTravelRandomThreaded()
        {
            Thread[] t = new Thread[5];
            for (int i = 0; i < t.Length; i++)
            {
                t[i] = new Thread(new ThreadStart(this.TestTravelRandom));
                t[i].Start();
            }
            for (int i = 0; i < t.Length; i++)
            {
                t[i].Join();
            }
        }
    }
}
