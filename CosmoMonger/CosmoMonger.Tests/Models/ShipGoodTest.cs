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
    public class ShipGoodTest : BasePlayerTest
    {
        [Test]
        public void Sell()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);
            
            // Store the player starting cash
            int playerStartingCash = testPlayer.CashCredits;

            // Add some water to this ship for us to sell
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            ShipGood shipGood = new ShipGood();
            shipGood.Good = water;
            shipGood.Quantity = 10;
            shipGood.Ship = testShip;
            testShip.ShipGoods.Add(shipGood);

            // Verify that the good is for sell in the current system
            SystemGood systemWater = startingSystem.GetGood(water.GoodId);
            if (systemWater == null)
            {
                startingSystem.AddGood(water.GoodId, 0);
                systemWater = startingSystem.GetGood(water.GoodId);
            }
            int playerProfit = (int)(systemWater.PriceMultiplier * water.BasePrice);
            int systemStartingCount = systemWater.Quantity;
            shipGood.Sell(manager, 10);

            Assert.That(systemWater.Quantity, Is.EqualTo(systemStartingCount + 10), "System should now have 10 more water goods");
            Assert.That(testPlayer.CashCredits, Is.EqualTo(playerStartingCash + playerProfit), "Player should have more cash credits now after selling");
        }

        [Test]
        public void SellNotSold()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);

            // Store the player starting cash
            int playerStartingCash = testPlayer.CashCredits;

            // Add some water to this ship for us to sell
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            ShipGood shipGood = new ShipGood();
            shipGood.Good = water;
            shipGood.Quantity = 10;
            shipGood.Ship = testShip;
            testShip.ShipGoods.Add(shipGood);

            // Verify that the good is for sell in the current system
            SystemGood systemWater = startingSystem.GetGood(water.GoodId);
            if (systemWater != null)
            {
                // Remove the good from the system
                startingSystem.SystemGoods.Remove(systemWater);
            }
            try
            {
                shipGood.Sell(manager, 10);
            }
            catch (InvalidOperationException ex)
            {
                // Correct
                return;
            }
            Assert.Fail("Player should not been able to sell in the system when the good was not sold in the system");
        }

        [Test]
        public void SellNotEnoughGoods()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            // Setup player
            Player testPlayer = this.CreateTestPlayer();
            Ship testShip = testPlayer.Ship;
            CosmoSystem startingSystem = testShip.CosmoSystem;
            GameManager manager = new GameManager(testPlayer.User.UserName);

            // Store the player starting cash
            int playerStartingCash = testPlayer.CashCredits;

            // Add some water to this ship for us to sell
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            ShipGood shipGood = new ShipGood();
            shipGood.Good = water;
            shipGood.Quantity = 10;
            shipGood.Ship = testShip;
            testShip.ShipGoods.Add(shipGood);

            // Verify that the good is for sell in the current system
            SystemGood systemWater = startingSystem.GetGood(water.GoodId);
            if (systemWater == null)
            {
                startingSystem.AddGood(water.GoodId, 0);
                systemWater = startingSystem.GetGood(water.GoodId);
            }

            try
            {
                shipGood.Sell(manager, 20);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("quantity"), "Quantity to sell should be the invalid argument");
            }

            Assert.Fail("Player should not been able to sell more goods than aboard");
        }
    }
}
