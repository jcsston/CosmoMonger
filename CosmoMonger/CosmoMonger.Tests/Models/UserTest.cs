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
        private string baseTestUsername;
        private string baseTestEmail;

        public UserTest()
        {
            SetupNewTestData();
        }

        private void SetupNewTestData()
        {
            string postFix = DateTime.Now.ToBinary().ToString();
            baseTestUsername = "testUser_" + postFix;
            baseTestEmail = "testUser_" + postFix + "@cosmomonger.com";
        }

        [Test]
        public void UserCreatePlayer()
        {
            SetupNewTestData();
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser("create" + this.baseTestUsername, "test1000", "create" + this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User is created");
            
            Assert.AreEqual("create" + this.baseTestUsername, testUser.UserName, "Test User has correct username");
            Assert.AreEqual("create" + this.baseTestEmail, testUser.Email, "Test User has correct e-mail");

            testUser = (CosmoMongerMembershipUser)provider.GetUser("create"+ this.baseTestUsername, false);
            Assert.IsNotNull(testUser, "Test User exists in the database");
            Assert.AreEqual("create" + this.baseTestUsername, testUser.UserName, "Test User has correct username");
            Assert.AreEqual("create" + this.baseTestEmail, testUser.Email, "Test User has correct e-mail");

            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            Assert.IsNotNull(humanRace, "Human Race exists in database");
            
            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");
            
            Player testPlayer = testUserModel.CreatePlayer("player" + this.baseTestUsername, humanRace);
            Assert.IsNotNull(testPlayer, "Test Player is created");
            
            Assert.AreEqual(true, testPlayer.Alive, "Test Player is alive");
            Assert.AreEqual("player" + this.baseTestUsername, testPlayer.Name, "Test Player has correct name");
        }

        [Test]
        public void UserUpdateProfile1()
        {
            SetupNewTestData();
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created");

            // Test the updating of just the username
            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");
            testUserModel.UpdateProfile("update" + baseTestUsername, this.baseTestEmail);

            testUser = (CosmoMongerMembershipUser)provider.GetUser("update" + this.baseTestUsername, false);
            Assert.IsNotNull(testUser, "Test User with updated username exists in the database");
            Assert.AreEqual("update" + this.baseTestUsername, testUser.UserName, "Test User actually has updated username");
            Assert.AreEqual(this.baseTestEmail, testUser.Email, "Test User actually has orignal e-mail");
        }

        [Test]
        public void UserUpdateProfile2()
        {
            SetupNewTestData();
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created");

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            // Test the updating of the e-mail
            testUserModel.UpdateProfile(this.baseTestUsername, "update" + this.baseTestEmail);

            string usernameWithUpdatedEmail = provider.GetUserNameByEmail("update" + this.baseTestEmail);
            Assert.AreEqual(this.baseTestUsername, usernameWithUpdatedEmail, "Test User actually has updated e-mail");
        }

        [Test]
        public void UserUpdateProfile3()
        {
            SetupNewTestData();
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created");

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            // Test the updating of both username and the e-mail
            testUserModel.UpdateProfile("update" + this.baseTestUsername, "update" + this.baseTestEmail);

            string usernameWithUpdatedEmail = provider.GetUserNameByEmail("update" + this.baseTestEmail);
            Assert.AreEqual("update" + this.baseTestUsername, usernameWithUpdatedEmail, "Test User actually has updated username");
        }

        [Test]
        public void UserUpdateProfile4()
        {
            SetupNewTestData();
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(this.baseTestUsername, "test1000", this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created");

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            // Change both to something else
            testUserModel.UpdateProfile("update2" + this.baseTestUsername, "update2" + this.baseTestEmail);

            // Test updating with the orignal profile values
            testUserModel.UpdateProfile(this.baseTestUsername, this.baseTestEmail);

            testUser = (CosmoMongerMembershipUser)provider.GetUser(this.baseTestUsername, false);
            Assert.IsNotNull(testUser, "Reverted Test User exists in the database");
            Assert.AreEqual(this.baseTestUsername, testUser.UserName, "Test User actually has orignal username");
            Assert.AreEqual(this.baseTestEmail, testUser.Email, "Test User actually has orignal e-mail");
        }

        /// <summary>
        /// Tests the send message.
        /// </summary>
        [Test]
        public void UserSendMessage()
        {
            SetupNewTestData();
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser1 = (CosmoMongerMembershipUser)provider.CreateUser("msg1" + this.baseTestUsername, "test1000", "msg1" + this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser1, "Test User 1 was created");

            User testUserModel1 = testUser1.GetUserModel();
            Assert.IsNotNull(testUserModel1, "Able to get model object for user 1");

            CosmoMongerMembershipUser testUser2 = (CosmoMongerMembershipUser)provider.CreateUser("msg2" + this.baseTestUsername, "test1000", "msg2" + this.baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser2, "Test User 2 was created");

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
