namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using CosmoMonger.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    

    /// <summary>
    /// Summary description for UserTest
    /// </summary>
    [TestClass]
    public class UserTest
    {
        private string baseTestUsername;
        private string baseTestEmail;

        public UserTest()
        {
            string postFix = DateTime.Now.ToBinary().ToString();
            baseTestUsername = "testUser_" + postFix;
            baseTestEmail = "testUser_" + postFix + "@cosmomonger.com";
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
        public void TestCreatePlayer()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            UserManager userManager = new UserManager();
            User testUser = userManager.CreateUser("create" + this.baseTestUsername, "test1000", "create" + this.baseTestEmail);
            Assert.IsNotNull(testUser, "Test User is created");
            Assert.AreEqual("create" + this.baseTestUsername, testUser.UserName, "Test User has correct username");
            Assert.AreEqual("create" + this.baseTestEmail, testUser.Email, "Test User has correct e-mail");

            testUser = userManager.GetUserByUserName("create"+ this.baseTestUsername);
            Assert.IsNotNull(testUser, "Test User exists in the database");
            Assert.AreEqual("create" + this.baseTestUsername, testUser.UserName, "Test User has correct username");
            Assert.AreEqual("create" + this.baseTestEmail, testUser.Email, "Test User has correct e-mail");

            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            Assert.IsNotNull(humanRace, "Human Race exists in database");
            Player testPlayer = testUser.CreatePlayer("player" + this.baseTestUsername, humanRace);
            Assert.IsNotNull(testPlayer, "Test Player is created");
            Assert.AreEqual(true, testPlayer.Alive, "Test Player is alive");
            Assert.AreEqual("player" + this.baseTestUsername, testPlayer.Name, "Test Player has correct name");
        }

        [TestMethod]
        public void TestUpdateProfile()
        {
            UserManager userManager = new UserManager();
            User testUser = userManager.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail);
            Assert.IsNotNull(testUser, "Test User exists in the database");
            Assert.AreEqual(testUser.UserName, this.baseTestUsername, "Test User has correct username");
            Assert.AreEqual(testUser.Email, this.baseTestEmail, "Test User has correct e-mail");

            // Test the updating of just the username
            testUser.UpdateProfile("update"+baseTestUsername, testUser.Email);

            testUser = userManager.GetUserByUserName("update" + this.baseTestUsername);
            Assert.IsNotNull(testUser, "Test User with updated username exists in the database");
            Assert.AreEqual(testUser.UserName, "update" + this.baseTestUsername, "Test User actually has updated username");
            Assert.AreEqual(testUser.Email, this.baseTestEmail, "Test User actually has orignal e-mail");


            // Test the updating of the e-mail
            testUser.UpdateProfile(testUser.UserName, "update" + this.baseTestEmail);

            testUser = userManager.GetUserByEmail("update" + this.baseTestEmail);
            Assert.IsNotNull(testUser, "Test User with updated e-mail exists in the database");
            Assert.AreEqual(testUser.UserName, "update" + this.baseTestUsername, "Test User actually has updated username");
            Assert.AreEqual(testUser.Email, "update" + this.baseTestEmail, "Test User actually has updated e-mail");


            // Test the updating of both username and the e-mail
            testUser.UpdateProfile("update" + this.baseTestUsername, "update" + this.baseTestEmail);

            testUser = userManager.GetUserByEmail("update" + this.baseTestEmail);
            Assert.IsNotNull(testUser, "Test User with updated e-mail exists in the database");
            Assert.AreEqual(testUser.UserName, "update" + this.baseTestUsername, "Test User actually has updated username");
            Assert.AreEqual(testUser.Email, "update" + this.baseTestEmail, "Test User actually has updated e-mail");


            // Test updating back to the orignal profile values
            testUser.UpdateProfile(this.baseTestUsername, this.baseTestEmail);

            testUser = userManager.GetUserByUserName(this.baseTestUsername);
            Assert.IsNotNull(testUser, "Reverted Test User exists in the database");
            Assert.AreEqual(testUser.UserName, this.baseTestUsername, "Test User actually has orignal username");
            Assert.AreEqual(testUser.Email, this.baseTestEmail, "Test User actually has orignal e-mail");
        }

        /// <summary>
        /// Tests the send message.
        /// </summary>
        [TestMethod]
        public void TestSendMessage()
        {
            UserManager userManager = new UserManager();
            User testUser1 = userManager.CreateUser("msg1" + this.baseTestUsername, "test1000", "msg1" + this.baseTestEmail);
            Assert.IsNotNull(testUser1, "Test User 1 is created");

            User testUser2 = userManager.CreateUser("msg2" + this.baseTestUsername, "test1000", "msg2" + this.baseTestEmail);
            Assert.IsNotNull(testUser2, "Test User 2 is created");

            for (int i = 0; i < 10; i++)
            {
                testUser1.SendMessage(testUser2, "Hello world!");
            }

            Message [] messages = testUser1.FetchUnreadMessages();
            foreach (Message msg in messages)
            {
                Assert.AreEqual(msg.RecipientUserId, testUser1.UserId, "Recipient user should match the user the message is stored under");
                Assert.AreEqual(msg.SenderUserId, testUser2.UserId, "Sender user should match the user the message from");
                Assert.AreEqual(msg.Content, "Hello world!", "Message should match what we sent");
                Assert.IsFalse(msg.Received, "This message should not be read yet.");
                msg.MarkAsReceived();
                Assert.IsTrue(msg.Received, "This message should now be read.");
            }
        }
    }
}
