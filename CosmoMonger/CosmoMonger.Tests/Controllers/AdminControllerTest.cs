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
    public class AdminControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.Admin)
                .Returns(true).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            AdminController controller = new AdminController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void IndexNotAdmin()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.Admin)
                .Returns(false).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            AdminController controller = new AdminController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void BanUser()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.Admin)
                .Returns(true).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Mock<User> banUserMock = new Mock<User>();
            banUserMock.Expect(u => u.Ban())
                .AtMostOnce().Verifiable();
            banUserMock.Expect(u => u.UserName)
                .Returns("BadUser").AtMostOnce().Verifiable();
            managerMock.Expect(m => m.GetUser(5))
                .Returns(banUserMock.Object).AtMostOnce().Verifiable();

            AdminController controller = new AdminController(managerMock.Object);

            // Act
            ActionResult result = controller.BanUser(5);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["UserName"], Is.EqualTo("BadUser"), "Banned user should be BadUser");
            managerMock.Verify();
        }

        [Test]
        public void FindUser()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.Admin)
                .Returns(true).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.FindUser("search string"))
                .Returns((new User[0]).AsQueryable<User>()).AtMostOnce().Verifiable();
            AdminController controller = new AdminController(managerMock.Object);

            // Act
            ActionResult result = controller.FindUser("search string", null);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["name"], Is.EqualTo("search string"), "name should be search string");
            Assert.That(viewResult.ViewData["Matches"], Is.InstanceOfType(typeof(IEnumerable<User>)), "Should be an enumerable collection of matching users");
            managerMock.Verify();
        }

        [Test]
        public void UnbanUser()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.Admin)
                .Returns(true).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Mock<User> banUserMock = new Mock<User>();
            banUserMock.Expect(u => u.Unban())
                .AtMostOnce().Verifiable();
            banUserMock.Expect(u => u.UserName)
                .Returns("GoodUser").AtMostOnce().Verifiable();
            managerMock.Expect(m => m.GetUser(5))
                .Returns(banUserMock.Object).AtMostOnce().Verifiable();

            AdminController controller = new AdminController(managerMock.Object);

            // Act
            ActionResult result = controller.UnbanUser(5);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["UserName"], Is.EqualTo("GoodUser"), "Unbanned user should be GoodUser");
            managerMock.Verify();
        }
    }
}
