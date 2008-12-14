using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CosmoMonger.Models;
using System.Web.Security;

namespace CosmoMonger.Tests.Models
{
    /// <summary>
    /// Summary description for UserTest
    /// </summary>
    [TestClass]
    public class UserTest
    {
        public UserTest()
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

        [TestMethod]
        public void TestCreatePlayer()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            MembershipUser user = provider.CreateUser("testUser", "test1000", "test@cosmomonger.com", null, null, true, null, out status);
            Assert.AreEqual(MembershipCreateStatus.Success, status, "Test User is created");

            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User testUser = (from u in db.Users where u.UserName == "testUser" select u).SingleOrDefault();
            Assert.IsNotNull(testUser, "Test User exists in the database");
        }

        [TestMethod]
        public void TestUpdateProfile()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User testUser = (from u in db.Users where u.UserName == "testUser" select u).SingleOrDefault();
            Assert.IsNotNull(testUser, "Test User exists in the database");

            testUser.UpdateProfile("testUser1", "test1@cosmomonger.com");

            User testUser1 = (from u in db.Users where u.UserName == "testUser1" select u).SingleOrDefault();
            Assert.IsNotNull(testUser1, "Updated Test User exists in the database");

            testUser1.UpdateProfile("testUser", "test@cosmomonger.com");

            User testUser2 = (from u in db.Users where u.UserName == "testUser" select u).SingleOrDefault();
            Assert.IsNotNull(testUser1, "Reverted Test User exists in the database");
        }

        [TestMethod]
        public void TestSendMessage()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User testUser = (from u in db.Users where u.UserName == "testUser" select u).SingleOrDefault();
            Assert.IsNotNull(testUser, "Test User exists in the database");

            User testUser1 = (from u in db.Users where u.UserName == "testUser1" select u).SingleOrDefault();
            Assert.IsNotNull(testUser1, "Updated Test User exists in the database");

            for (int i = 0; i < 100; i++)
            {
                testUser.SendMessage(testUser1, "Hello world!");
            }

            Message [] messages = testUser.FetchUnreadMessages();
            foreach (Message msg in messages)
            {
                Assert.AreEqual(msg.RecipientUserId, testUser.UserId, "Recipient user should match the user the message is stored under");
                Assert.AreEqual(msg.SenderUserId, testUser1.UserId, "Sender user should match the user the message from");
                Assert.AreEqual(msg.Content, "Hello world!", "Message should match what we sent");
                Assert.IsFalse(msg.Received, "This message should not be read yet.");
                msg.MarkAsReceived();
                Assert.IsTrue(msg.Received, "This message should now be read.");
            }
        }
    }
}
