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

    [TestFixture]
    public class SystemGoodTest
    {
        private string baseTestUsername = "testUser";
        private string baseTestEmail = "testUser@cosmomonger.com";
        private string baseTestPlayerName = "testPlayer";

        [SetUp]
        [TearDown]
        public void Cleanup()
        {
            // Cleanup any possible test players
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            provider.DeleteUser(this.baseTestUsername, true);
        }

        [Test]
        public void Buy()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            // Setup player
            Player testPlayer = PlayerTest.CreateTestPlayer(this.baseTestUsername, this.baseTestEmail, this.baseTestPlayerName);
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(this.baseTestUsername);

            // Check that the ship is empty
            Assert.That(testShip.ShipGoods.Count, Is.EqualTo(0), "Ship should start out with no goods on-board");

            // Add some water to the starting system for this ship to buy
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            startingSystem.AddGood(water.GoodId, 20);

            // Verify that the good was added to the system
            SystemGood systemWater = startingSystem.GetGood(water.GoodId);
            Assert.That(systemWater, Is.Not.Null, "System should now have a water SystemGood");
            Assert.That(systemWater.Quantity, Is.GreaterThanOrEqualTo(100), "System should now have at least 20 water goods");

            int systemStartingCount = systemWater.Quantity;
            systemWater.Buy(manager, 20);

            Assert.That(systemWater.Quantity, Is.EqualTo(systemStartingCount - 20), "System should now have 20 few water goods");
        }
    }
}
