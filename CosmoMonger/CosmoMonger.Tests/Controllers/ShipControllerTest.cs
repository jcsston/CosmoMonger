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
    public class ShipControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            ShipController controller = new ShipController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void ListShips()
        {
            // Arrange
            SystemShip[] ships = { new SystemShip(), new SystemShip() };
            
            /*
            CosmoSystem system = new CosmoSystem();
            ships[0].CosmoSystem = system;
            ships[0].PriceMultiplier = 1.0;
            ships[0].Quantity = 1;
            ships[0].BaseShip = new BaseShip();
            ships[0].BaseShip.Name = "Ship 1";
            ships[0].BaseShip.BasePrice = 1000;
            
            ships[1].CosmoSystem = system;
            ships[1].PriceMultiplier = 1.0;
            ships[1].Quantity = 1;
            ships[1].BaseShip = new BaseShip();
            ships[1].BaseShip.Name = "Ship 2";
            ships[1].BaseShip.BasePrice = 1000;
            */

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetAvailableShips())
                .Returns(ships).AtMostOnce().Verifiable();
            ShipController controller = new ShipController(managerMock.Object);

            // Act
            ActionResult result = controller.ListShips();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Ships"], Is.EqualTo(ships), "The Ships field should be the array of ship objects");
            
            managerMock.Verify();
        }

        [Test]
        public void BuyShip()
        {
            // Arrange
            Mock<SystemShip> shipMock = new Mock<SystemShip>();
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);

            shipMock.Expect(m => m.Buy(managerMock.Object))
                .AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetShip(1))
                .Returns(shipMock.Object).AtMostOnce().Verifiable();
            ShipController controller = new ShipController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyShip(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }

        [Test]
        public void BuyShipInvalidId()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetShip(-5))
                .Returns<SystemShip>(null).AtMostOnce().Verifiable();
            ShipController controller = new ShipController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyShip(-5);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["shipId"].Errors, Is.Not.Empty, "ShipId should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void BuyShipFailedToBuy()
        {
            // Arrange
            Mock<SystemShip> shipMock = new Mock<SystemShip>();
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);

            shipMock.Expect(m => m.Buy(managerMock.Object))
                .Throws(new InvalidOperationException()).AtMostOnce().Verifiable();

            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.GetShip(5))
                .Returns(shipMock.Object).AtMostOnce().Verifiable();
            ShipController controller = new ShipController(managerMock.Object);

            // Act
            ActionResult result = controller.BuyShip(5);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "A error should be returned");
            Assert.That(controller.ModelState["_FORM"].Errors, Is.Not.Empty, "_FORM should be the error field");

            managerMock.Verify();
        }

        [Test]
        public void ViewShip()
        {
            // Arrange
            Ship ship = new Ship();
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship)
                .Returns(ship).Verifiable();
            ShipController controller = new ShipController(managerMock.Object);

            // Act
            ActionResult result = controller.ViewShip();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Ship"], Is.EqualTo(ship), "The Ship field should be the ship object");

            managerMock.Verify();
        }
    }
}
