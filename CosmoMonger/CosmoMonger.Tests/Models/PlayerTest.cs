namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using CosmoMonger.Models;
    using System.Web.Security;
    using System.Threading;

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

        static public Player CreateTestPlayer()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            string postFix = DateTime.Now.ToBinary().ToString() + "_thread" + Thread.CurrentThread.ManagedThreadId;
            string baseTestUsername = "testUser_" + postFix;
            string baseTestEmail = "testUser_" + postFix + "@cosmomonger.com";

            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(baseTestUsername, "test1000", baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created");

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            return testUserModel.CreatePlayer(baseTestUsername, humanRace);
        }

        [TestMethod]
        public void PlayerUpdateProfile()
        {
            // We first need to create a player
            Player testPlayer = PlayerTest.CreateTestPlayer();
            
            // Update profile
            testPlayer.UpdateProfile("player2" + this.baseTestUsername);
            // Check results
            Assert.AreEqual("player2" + this.baseTestUsername, testPlayer.Name, "Player name is updated");
        }
    }
}
