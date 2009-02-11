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
    public class CommunicationControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void Inbox()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.GetMessages())
                .Returns(new List<Message>()).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.Inbox(null);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["Messages"], Is.InstanceOfType(typeof(IEnumerable<Message>)), "Messages should be an IEnumerable of Message objects");
            managerMock.Verify();
        }

        [Test]
        public void Sent()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.GetMessagesSent())
                .Returns(new List<Message>()).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.Sent(null);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["Messages"], Is.InstanceOfType(typeof(IEnumerable<Message>)), "Messages should be an IEnumerable of Message objects");
            managerMock.Verify();
        }

        [Test]
        public void Compose()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.GetBuddyList())
                .Returns(new BuddyList[0]).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.Compose(null);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["toUserId"], Is.InstanceOfType(typeof(SelectList)), "toUserId should be an SelectList of buddies");
            managerMock.Verify();
        }

        [Test]
        public void ComposeToUser()
        {
            // Arrange
            Mock<User> toUserMock = new Mock<User>();
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.SendMessage(toUserMock.Object, "Test Subject", "Test Message"))
                .AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetUser(5))
                .Returns(toUserMock.Object).AtMostOnce().Verifiable();
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.Compose(5, "Test Subject", "Test Mesage");

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }

        [Test]
        public void ViewMessage()
        {
            // Arrange
            Mock<Message> messageMock = new Mock<Message>();
            Mock<User> toUserMock = new Mock<User>();
            toUserMock.Expect(u => u.UserName)
                .Returns("To username").Verifiable();

            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.UserName)
                .Returns("From username").Verifiable();

            // Setup message mock
            messageMock.Expect(m => m.RecipientUser)
                .Returns(toUserMock.Object).Verifiable();
            messageMock.Expect(m => m.SenderUser)
                .Returns(userMock.Object).Verifiable();
            messageMock.Expect(m => m.Subject)
                .Returns("Message Subject").Verifiable();
            DateTime messageDate = new DateTime(2009, 2, 10, 6, 54, 3);
            messageMock.Expect(m => m.Time)
                .Returns(messageDate).Verifiable();
            messageMock.Expect(m => m.Content)
                .Returns("Message Content").Verifiable();

            userMock.Expect(u => u.GetMessage(50))
                .Returns(messageMock.Object).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.ViewMessage(50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["From"], Is.EqualTo("From username"), "From field should match from user name");
            Assert.That(viewResult.ViewData["To"], Is.EqualTo("To username"), "To field should match target user name");
            Assert.That(viewResult.ViewData["Subject"], Is.EqualTo("Message Subject"), "Subject field should match message subject");
            Assert.That(viewResult.ViewData["Time"], Is.EqualTo(messageDate), "To field should match target user name");
            Assert.That(viewResult.ViewData["Content"], Is.EqualTo("Message Content"), "Content field should match message content");
            
            managerMock.Verify();
        }

        [Test]
        public void DeleteMessage()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.DeleteMessage(50))
                .AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.DeleteMessage(50);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }

        [Test]
        public void UnreadMessages()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(u => u.GetUnreadMessages())
                .Returns(new Message[3]).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CommunicationController controller = new CommunicationController(managerMock.Object);

            // Act
            ActionResult result = controller.UnreadMessages();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(JsonResult)), "Should return a JSON");
            JsonResult jsonResult = (JsonResult)result;
            Assert.That(jsonResult.Data, Is.EqualTo(3), "Should be the number of items in the Message array");

            managerMock.Verify();
        }
    }
}
