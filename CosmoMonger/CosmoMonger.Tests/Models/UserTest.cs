namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using CosmoMonger.Models;
    using NUnit.Framework;
    
    /// <summary>
    /// Summary description for UserTest
    /// </summary>
    [TestFixture]
    public class UserTest
    {
        private string baseTestUsername = "testUser";
        private string baseTestEmail = "testUser@cosmomonger.com";

        [SetUp]
        [TearDown]
        public void Cleanup()
        {
            // Cleanup any possible test users
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            provider.DeleteUser(this.baseTestUsername, true);
            provider.DeleteUser("1" + this.baseTestUsername, true);
            provider.DeleteUser("2" + this.baseTestUsername, true);
        }

        [Test]
        public void CreatePlayer()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User is created");
            
            Assert.AreEqual(this.baseTestUsername, testUser.UserName, "Test User has correct username");
            Assert.AreEqual(this.baseTestEmail, testUser.Email, "Test User has correct e-mail");

            testUser = (CosmoMongerMembershipUser)provider.GetUser(this.baseTestUsername, false);
            Assert.IsNotNull(testUser, "Test User exists in the database");
            Assert.AreEqual(this.baseTestUsername, testUser.UserName, "Test User has correct username");
            Assert.AreEqual(this.baseTestEmail, testUser.Email, "Test User has correct e-mail");

            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            Assert.IsNotNull(humanRace, "Human Race exists in database");
            
            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");
            
            Player testPlayer = testUserModel.CreatePlayer(this.baseTestUsername, humanRace);
            Assert.IsNotNull(testPlayer, "Test Player is created");
            
            Assert.AreEqual(true, testPlayer.Alive, "Test Player is alive");
            Assert.AreEqual(this.baseTestUsername, testPlayer.Name, "Test Player has correct name");
        }

        [Test]
        public void DeleteUser()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created. status = {0}", new object[]{status});

            // Test the deleting of the user
            bool deleted = provider.DeleteUser(this.baseTestUsername, true);
            Assert.IsTrue(deleted, "The DeleteUser method should have returned true");

            testUser = (CosmoMongerMembershipUser)provider.GetUser(this.baseTestUsername, false);
            Assert.IsNull(testUser, "Test User has been deleted from the database");
        }

        [Test]
        public void DeleteUserLeaveUserData()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created. status = {0}", new object[] { status });

            try
            {
                // Test the deleting of the user
                provider.DeleteUser(this.baseTestUsername, false);
                Assert.Fail("The DeleteUser method should have thrown an exception");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("deleteAllRelatedData", ex.ParamName, "Failed argument should be deleteAllRelatedData.");
            }

            testUser = (CosmoMongerMembershipUser)provider.GetUser(this.baseTestUsername, false);
            Assert.IsNotNull(testUser, "Test User should still be in the database");
        }

        [Test]
        public void UpdateProfile1()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created. status = {0}", new object[] { status });

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            // Test the updating of the e-mail
            testUserModel.UpdateEmail("1" + this.baseTestEmail);

            string usernameWithUpdatedEmail = provider.GetUserNameByEmail("1" + this.baseTestEmail);
            Assert.AreEqual(this.baseTestUsername, usernameWithUpdatedEmail, "Test User actually has updated e-mail");
        }

        [Test]
        public void UpdateProfile2()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created. status = {0}", new object[] { status });

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            // Change both to something else
            testUserModel.UpdateEmail("1" + this.baseTestEmail);

            // Test updating with the orignal profile values
            testUserModel.UpdateEmail(this.baseTestEmail);

            testUser = (CosmoMongerMembershipUser)provider.GetUser(this.baseTestUsername, false);
            Assert.IsNotNull(testUser, "Reverted Test User exists in the database");
            Assert.AreEqual(this.baseTestUsername, testUser.UserName, "Test User actually has orignal username");
            Assert.AreEqual(this.baseTestEmail, testUser.Email, "Test User actually has orignal e-mail");
        }

        /// <summary>
        /// Tests the send message.
        /// </summary>
        [Test]
        public void SendMessage()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser1 = (CosmoMongerMembershipUser)provider.CreateUser("1" + this.baseTestUsername, "test1000", "1" + this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser1, "Test User 1 was created. status = {0}", new object[] { status });

            User testUserModel1 = testUser1.GetUserModel();
            Assert.IsNotNull(testUserModel1, "Able to get model object for user 1");

            CosmoMongerMembershipUser testUser2 = (CosmoMongerMembershipUser)provider.CreateUser("2" + this.baseTestUsername, "test1000", "2" + this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser2, "Test User 2 was created. status = {0}", new object[] { status });

            User testUserModel2 = testUser2.GetUserModel();
            Assert.IsNotNull(testUserModel2, "Able to get model object for user 2");


            for (int i = 0; i < 10; i++)
            {
                testUserModel1.SendMessage(testUserModel2, "Hello world!");
            }

            Message[] messages = testUserModel1.FetchUnreadMessages();
            foreach (Message msg in messages)
            {
                Assert.AreEqual(msg.RecipientUserId, testUserModel1.UserId, "Recipient user should match the user the message is stored under");
                Assert.AreEqual(msg.SenderUserId, testUserModel2.UserId, "Sender user should match the user the message from");
                Assert.AreEqual(msg.Content, "Hello world!", "Message should match what we sent");
                Assert.IsFalse(msg.Received, "This message should not be read yet.");
                msg.MarkAsReceived();
                Assert.IsTrue(msg.Received, "This message should now be read.");
            }
        }
    }
}
