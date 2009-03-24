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
    public class BuddyListControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void BuddyList()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            BuddyList[] list = new BuddyList[0];
            userMock.Expect(u => u.GetBuddyList())
                .Returns(list)
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.BuddyList();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["BuddyList"], Is.EqualTo(list), "ViewData should have buddy list");
            managerMock.Verify();
        }

        [Test]
        public void FindPlayer()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Player[] list = new Player[0];
            managerMock.Expect(m => m.FindPlayer("test"))
                .Returns(list)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.FindPlayer("test", 1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["Matches"], Is.Empty, "ViewData should have empty player list");
            Assert.That(viewResult.ViewData["name"], Is.EqualTo("test"), "ViewData should have player search");
            managerMock.Verify();
        }

        [Test]
        public void AddBuddy()
        {
            // Arrange
            Mock<User> buddyUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.AddBuddy(buddyUserMock.Object))
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(1))
                .Returns(buddyUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.AddBuddy(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AddBuddyExisting()
        {
            // Arrange
            Mock<User> buddyUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.AddBuddy(buddyUserMock.Object))
                .Throws(new ArgumentException())
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(2))
                .Returns(buddyUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.AddBuddy(2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AddBuddyInvalid()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(3))
                .Returns<User>(null)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.AddBuddy(3);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void RemoveBuddy()
        {
            // Arrange
            Mock<User> buddyUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.RemoveBuddy(buddyUserMock.Object))
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(1))
                .Returns(buddyUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.RemoveBuddy(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void RemoveBuddyNotInList()
        {
            // Arrange
            Mock<User> buddyUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.RemoveBuddy(buddyUserMock.Object))
                .Throws(new ArgumentException())
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(2))
                .Returns(buddyUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.RemoveBuddy(2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void RemoveBuddyInvalid()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(3))
                .Returns<User>(null)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.RemoveBuddy(3);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AddIgnore()
        {
            // Arrange
            Mock<User> ignoreUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.AddIgnore(ignoreUserMock.Object))
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(1))
                .Returns(ignoreUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.AddIgnore(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AddIgnoreExisting()
        {
            // Arrange
            Mock<User> ignoreUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.AddIgnore(ignoreUserMock.Object))
                .Throws(new ArgumentException())
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(2))
                .Returns(ignoreUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.AddIgnore(2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AddIgnoreInvalid()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(3))
                .Returns<User>(null)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.AddIgnore(3);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void RemoveIgnore()
        {
            // Arrange
            Mock<User> ignoreUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.RemoveIgnore(ignoreUserMock.Object))
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(1))
                .Returns(ignoreUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.RemoveIgnore(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void RemoveIgnoreNotInList()
        {
            // Arrange
            Mock<User> ignoreUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.RemoveIgnore(ignoreUserMock.Object))
                .Throws(new ArgumentException())
                .Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(2))
                .Returns(ignoreUserMock.Object)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.RemoveIgnore(2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void RemoveIgnoreInvalid()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(3))
                .Returns<User>(null)
                .Verifiable();
            BuddyListController controller = new BuddyListController(managerMock.Object);

            // Act
            ActionResult result = controller.RemoveIgnore(3);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }
    }
}
