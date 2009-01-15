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
    /// This is the base test class for player related tests.
    /// This class will create a test player to work with and also cleanup the player when done.
    /// </summary>
    public class BasePlayerTest
    {
        private string baseTestUsername = "testUser";
        private string baseTestEmail = "testUser@cosmomonger.com";
        private string baseTestPlayerName = "testPlayer";
        private int playerCount = 0;

        protected Player CreateTestPlayer()
        {
            playerCount++;
            return CreateTestPlayer(playerCount + this.baseTestUsername, playerCount + this.baseTestEmail, this.baseTestPlayerName);
        }

        protected Player CreateTestPlayer(Race playerRace)
        {
            playerCount++;
            return CreateTestPlayer(playerCount + this.baseTestUsername, playerCount + this.baseTestEmail, this.baseTestPlayerName, playerRace);
        }

        private Player CreateTestPlayer(string baseTestUsername, string baseTestEmail, string baseTestPlayerName)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Default to Human race
            Race humanRace = (from r in db.Races
                              where r.Name == "Human"
                              select r).SingleOrDefault();
            Assert.IsNotNull(humanRace, "Human Race needs to be present in the database");

            return this.CreateTestPlayer(baseTestUsername, baseTestEmail, baseTestPlayerName, humanRace);
        }

        private Player CreateTestPlayer(string baseTestUsername, string baseTestEmail, string baseTestPlayerName, Race playerRace)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            CosmoMongerMembershipUser testUser = (CosmoMongerMembershipUser)provider.CreateUser(baseTestUsername, "test1000", baseTestEmail, null, null, true, null, out status);
            Assert.IsNotNull(testUser, "Test User was created. status = {0}", new object[] { status });

            User testUserModel = testUser.GetUserModel();
            Assert.IsNotNull(testUserModel, "Able to get model object for user");

            return testUserModel.CreatePlayer(baseTestUsername, playerRace);
        }

        [SetUp]
        [TearDown]
        public void Cleanup()
        {
            // Cleanup any possible test players
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            int totalRecords = 0;
            MembershipUserCollection col = provider.FindUsersByName(this.baseTestUsername, 0, 1000, out totalRecords);
            Assert.That(totalRecords, Is.LessThanOrEqualTo(1000), "BasePlayerTest class coded to only delete 1000 test users");
            foreach (MembershipUser user in col)
            {
                provider.DeleteUser(user.UserName, true);
            }
        }
    }
}
