namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using CosmoMonger.Models;

    /// <summary>
    /// Summary description for PlayerTest
    /// </summary>
    [TestClass]
    public class PlayerTest
    {
        private string baseTestUsername;
        private string baseTestEmail;
        private string baseTestPlayerName;

        public PlayerTest()
        {
            string postFix = DateTime.Now.ToBinary().ToString();
            this.baseTestUsername = "testUser_" + postFix;
            this.baseTestEmail = "testUser_" + postFix + "@cosmomonger.com";
            this.baseTestPlayerName = "testPlayer_" + postFix;
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

        [TestMethod]
        public void TestUpdateProfile()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            UserManager userManager = new UserManager();

            // We first need to create a player
            User testUser = userManager.CreateUser("create" + this.baseTestUsername, "test1000", "create" + this.baseTestEmail);
            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            Player testPlayer = testUser.CreatePlayer("player" + this.baseTestUsername, humanRace);
            
            // Update profile
            testPlayer.UpdateProfile("player2" + this.baseTestUsername);
            // Check results
            Assert.AreEqual("player2" + this.baseTestUsername, testPlayer.Name, "Player name is updated");
        }
    }
}
