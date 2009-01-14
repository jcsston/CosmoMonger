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
    public class SystemGoodTest : BasePlayerTest
    {
        [Test]
        public void Buy()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);

            // Store the player starting cash
            int playerStartingCash = testPlayer.CashCredits;

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
            Assert.That(systemWater.Quantity, Is.GreaterThanOrEqualTo(20), "System should now have at least 20 water goods");

            int playerCost = systemWater.Price * 20;
            int systemStartingCount = systemWater.Quantity;
            systemWater.Buy(manager, 20);

            Assert.That(systemWater.Quantity, Is.EqualTo(systemStartingCount - 20), "System should now have 20 few water goods");
            Assert.That(testPlayer.CashCredits, Is.EqualTo(playerStartingCash - playerCost), "Player should have less cash credits now after buying");
        }

        [Test]
        public void BuyNotEnoughGoods()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);

            // Check that the ship is empty
            Assert.That(testShip.ShipGoods.Count, Is.EqualTo(0), "Ship should start out with no goods on-board");

            // Add some water to the starting system for this ship to buy
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            startingSystem.AddGood(water.GoodId, 5);

            // Verify that the good was added to the system
            SystemGood systemWater = startingSystem.GetGood(water.GoodId);
            
            // Make sure only 5 are at the system
            systemWater.Quantity = 5;

            try
            {
                systemWater.Buy(manager, 20);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("quantity"), "Quantity to buy should be the invalid argument");
                return;
            }

            Assert.Fail("Player should not been able to buy more goods than at the system");
        }

        [Test]
        public void BuyNotEnoughCash()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);

            // Reduce the player starting cash
            testPlayer.CashCredits = 10;

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
            Assert.That(systemWater.Quantity, Is.GreaterThanOrEqualTo(20), "System should now have at least 20 water goods");

            int playerCost = (int)(systemWater.PriceMultiplier * water.BasePrice) * systemWater.Quantity;
            int systemStartingCount = systemWater.Quantity;
            try
            {
                systemWater.Buy(manager, 20);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("quantity"), "Quantity to buy should be the invalid argument");
                return;
            }

            Assert.Fail("Player should not been able to buy more goods than they can afford");
        }

        [Test]
        public void BuyNotEnoughSpace()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);

            // Make the player super rich to afford buying lots of goods
            testPlayer.CashCredits = 1000000000;

            // Check that the ship is empty
            Assert.That(testShip.ShipGoods.Count, Is.EqualTo(0), "Ship should start out with no goods on-board");

            // Add some water to the starting system for this ship to buy
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            startingSystem.AddGood(water.GoodId, 200);

            // Verify that the good was added to the system
            SystemGood systemWater = startingSystem.GetGood(water.GoodId);
            Assert.That(systemWater, Is.Not.Null, "System should now have a water SystemGood");
            Assert.That(systemWater.Quantity, Is.GreaterThanOrEqualTo(200), "System should now have at least 20 water goods");

            int playerCost = systemWater.Price * 2000;
            int systemStartingCount = systemWater.Quantity;
            try
            {
                systemWater.Buy(manager, 2000);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("quantity"), "Quantity to buy should be the invalid argument");
                return;
            }

            Assert.Fail("Player should not been able to buy more goods than they can carry");
        }
    }
}
