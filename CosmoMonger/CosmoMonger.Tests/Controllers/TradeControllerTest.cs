namespace CosmoMonger.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using CosmoMonger.Controllers;
    using CosmoMonger.Models;
    using Moq;
    using Moq.Mvc;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class TradeControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void ListGoods()
        {
            // Arrange
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };
            
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGoods())
                .Returns(goods).AtMostOnce().Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetGoods())
                .Returns(shipGoods).AtMostOnce().Verifiable();

            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.ListGoods();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["SystemGoods"], Is.EqualTo(goods), "The SystemGoods field should be the array of SystemGood objects");
            Assert.That(controller.ViewData["ShipGoods"], Is.EqualTo(shipGoods), "The ShipGoods field should be the array of ShipGood objects");
            
            managerMock.Verify();
        }

        [Test]
        public void BuyGood()
        {
            // Arrange
            Mock<SystemGood> goodMock = new Mock<SystemGood>();
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);

            goodMock.Expect(m => m.Buy(managerMock.Object, 10, 50))
                .AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyGoods(1, 10, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }

        [Test]
        public void BuyGoodInvalidId()
        {
            // Arrange
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGood(-5))
                .Returns<SystemGood>(null).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyGoods(-5, 10, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["goodId"].Errors, Is.Not.Empty, "goodId should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void BuyGoodTooMany()
        {
            // Arrange
            Mock<SystemGood> goodMock = new Mock<SystemGood>();
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            goodMock.Expect(m => m.Buy(managerMock.Object, 100, 50))
                .Throws(new ArgumentOutOfRangeException()).AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyGoods(1, 100, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["quantity"].Errors, Is.Not.Empty, "quantity should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void BuyGoodPriceChange()
        {
            // Arrange
            Mock<SystemGood> goodMock = new Mock<SystemGood>();
            goodMock.Expect(g => g.Price)
                .Returns(51).Verifiable();
            goodMock.Expect(g => g.Good.Name)
                .Returns("Good Name").Verifiable();

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            goodMock.Expect(m => m.Buy(managerMock.Object, 100, 50))
                .Throws(new ArgumentOutOfRangeException("price", 50, "Price change on good")).AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyGoods(1, 100, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("BuyGoodsPriceChange"), "The Price change view should be returned");
            Assert.That(viewResult.ViewData["goodId"], Is.EqualTo(1), "ViewData should have the correct good id");
            Assert.That(viewResult.ViewData["goodName"], Is.EqualTo("Good Name"), "ViewData should have the good name");
            Assert.That(viewResult.ViewData["oldPrice"], Is.EqualTo(50), "ViewData should have the old price");
            Assert.That(viewResult.ViewData["newPrice"], Is.EqualTo(51), "ViewData should have the new price");
            Assert.That(viewResult.ViewData["quantity"], Is.EqualTo(100), "ViewData should have the quantity");
            Assert.That(controller.ModelState.IsValid, Is.True, "No error should be returned");

            managerMock.Verify();
        }

        [Test]
        public void BuyGoodNotEnoughCargoCredits()
        {
            // Arrange
            Mock<SystemGood> goodMock = new Mock<SystemGood>();
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            goodMock.Expect(m => m.Buy(managerMock.Object, 100, 50))
                .Throws(new ArgumentException()).AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyGoods(1, 100, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["quantity"].Errors, Is.Not.Empty, "quantity should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void SellGood()
        {
            // Arrange
            Mock<ShipGood> goodMock = new Mock<ShipGood>();
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            goodMock.Expect(m => m.Sell(managerMock.Object, 10, 50))
                .AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.SellGoods(1, 10, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }

        [Test]
        public void SellGoodInvalidId()
        {
            // Arrange
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetGood(-5))
                .Returns<ShipGood>(null).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.SellGoods(-5, 10, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["goodId"].Errors, Is.Not.Empty, "goodId should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void SellGoodTooMany()
        {
            // Arrange
            Mock<ShipGood> goodMock = new Mock<ShipGood>();
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);

            goodMock.Expect(m => m.Sell(managerMock.Object, 100, 50))
                .Throws(new ArgumentOutOfRangeException()).AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.SellGoods(1, 100, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["quantity"].Errors, Is.Not.Empty, "quantity should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void SellGoodNotSold()
        {
            // Arrange
            Mock<ShipGood> goodMock = new Mock<ShipGood>();
            SystemGood[] goods = { new SystemGood(), new SystemGood() };
            ShipGood[] shipGoods = { new ShipGood(), new ShipGood() };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);

            goodMock.Expect(m => m.Sell(managerMock.Object, 10, 50))
                .Throws(new InvalidOperationException()).AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.SellGoods(1, 10, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["goodId"].Errors, Is.Not.Empty, "goodId should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void SellGoodPriceChange()
        {
            // Arrange
            Mock<SystemGood> goodMock = new Mock<SystemGood>();
            goodMock.Expect(g => g.Price)
                .Returns(51).Verifiable();

            Mock<ShipGood> shipGoodMock = new Mock<ShipGood>();

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetGood(1))
                 .Returns(shipGoodMock.Object).AtMostOnce().Verifiable();

            shipGoodMock.Expect(g => g.Sell(managerMock.Object, 100, 50))
                .Throws(new ArgumentOutOfRangeException("price", 50, "Price change on good"))
                .AtMostOnce().Verifiable();
 
            shipGoodMock.Expect(g => g.Good.Name)
                .Returns("Good Name").Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetGood(1))
                .Returns(goodMock.Object).AtMostOnce().Verifiable();
            TradeController controller = new TradeController(managerMock.Object);

            // Act
            ActionResult result = controller.SellGoods(1, 100, 50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("SellGoodsPriceChange"), "The Price change view should be returned");
            Assert.That(viewResult.ViewData["goodId"], Is.EqualTo(1), "ViewData should have the correct good id");
            Assert.That(viewResult.ViewData["goodName"], Is.EqualTo("Good Name"), "ViewData should have the good name");
            Assert.That(viewResult.ViewData["oldPrice"], Is.EqualTo(50), "ViewData should have the old price");
            Assert.That(viewResult.ViewData["newPrice"], Is.EqualTo(51), "ViewData should have the new price");
            Assert.That(viewResult.ViewData["quantity"], Is.EqualTo(100), "ViewData should have the quantity");
            Assert.That(controller.ModelState.IsValid, Is.True, "No error should be returned");

            managerMock.Verify();
        }
    }
}
