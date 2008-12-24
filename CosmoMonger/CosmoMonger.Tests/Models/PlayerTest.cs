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
    /// Summary description for PlayerTest
    /// </summary>
    [TestFixture]
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

        [Test]
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
