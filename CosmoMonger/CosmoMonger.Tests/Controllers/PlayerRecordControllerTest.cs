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
    public class PlayerRecordControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void ListRecords()
        {
            // Arrange
            Player [] topPlayers = { new Player(), new Player() };
            topPlayers[0].Name = "Player 1";
            topPlayers[1].Name = "Player 2";
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetTopPlayers("NetWorth", 10))
                .Returns(topPlayers).AtMostOnce().Verifiable();
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.ListRecords("NetWorth");

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["RecordTypes"], Is.TypeOf(typeof(string[])), "The RecordTypes field should be a string array");
            Assert.That(controller.ViewData["TopRecords"], Is.EqualTo(topPlayers), "The TopRecords field should be the array of top player objects");
            
            managerMock.Verify();
        }

        [Test]
        public void ViewRecord()
        {
            // Arrange
            Player player = new Player();
            player.PlayerId = 1;
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer)
                .Returns(player).Verifiable();
            managerMock.Expect(m => m.GetPlayer(1))
                .Returns(player).AtMostOnce().Verifiable();
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.ViewRecord();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Player"], Is.EqualTo(player), "The Player field should be the player object");

            managerMock.Verify();
        }

        [Test]
        public void ViewRecordDirect()
        {
            // Arrange
            Player player = new Player();
            player.PlayerId = 1;
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetPlayer(1))
                .Returns(player).AtMostOnce().Verifiable();
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.ViewRecord(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Player"], Is.EqualTo(player), "The Player field should be the player object");

            managerMock.Verify();
        }
    }
}
