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
    public class BankControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void Bank()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Name)
                .Returns("Player Name").Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem.HasBank)
                .Returns(true).Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Bank();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["PlayerName"], Is.EqualTo("Player Name"), "The PlayerName field should have a reference to the player name");
            Assert.That(controller.ViewData["BankAvailable"], Is.True, "The BankAvailable field should be true");
            managerMock.Verify();
        }

        [Test]
        public void WithdrawSucessful()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankWithdraw(1000))
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);
            
            // Act
            ActionResult result = controller.Withdraw(1000);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void WithdrawNoBank()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankWithdraw(500))
                .Throws<InvalidOperationException>()
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Withdraw(500);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["_FORM"].Errors, Is.Not.Empty, "Errors should be flaged on the form");
            managerMock.Verify();
        }

        [Test]
        public void WithdrawNotEnoughCredits()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankWithdraw(2500))
                .Throws<ArgumentOutOfRangeException>()
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Withdraw(2500);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["withdrawCredits"].Errors, Is.Not.Empty, "Errors should be flaged on the withdraw credits field");
            managerMock.Verify();
        }

        [Test]
        public void WithdrawNegativeCredits()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankWithdraw(-200))
                .Throws<ArgumentOutOfRangeException>()
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Withdraw(-200);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["withdrawCredits"].Errors, Is.Not.Empty, "Errors should be flaged on the withdraw credits field");
            managerMock.Verify();
        }

        [Test]
        public void DepositSucessful()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankDeposit(1000))
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Deposit(1000);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void DepositNoBank()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankDeposit(500))
                .Throws<InvalidOperationException>()
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Deposit(500);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["_FORM"].Errors, Is.Not.Empty, "Errors should be flaged on the form");
            managerMock.Verify();
        }

        [Test]
        public void DepositNotEnoughCredits()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankDeposit(2500))
                .Throws<ArgumentOutOfRangeException>()
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Deposit(2500);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["depositCredits"].Errors, Is.Not.Empty, "Errors should be flaged on the deposit credits field");
            managerMock.Verify();
        }

        [Test]
        public void DepositNegativeCredits()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.BankDeposit(-200))
                .Throws<ArgumentOutOfRangeException>()
                .AtMostOnce().Verifiable();
            BankController controller = new BankController(managerMock.Object);

            // Act
            ActionResult result = controller.Deposit(-200);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["depositCredits"].Errors, Is.Not.Empty, "Errors should be flaged on the deposit credits field");
            managerMock.Verify();
        }
    }
}
