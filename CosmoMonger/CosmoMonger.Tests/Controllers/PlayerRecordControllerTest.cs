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
            KeyValuePair<Player, string>[] topPlayers = { new KeyValuePair<Player, string>(new Player(), "0"), new KeyValuePair<Player, string>(new Player(), "1") };
            topPlayers[0].Key.Name = "Player 1";
            topPlayers[1].Key.Name = "Player 2";
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetTopPlayers(Player.RecordType.NetWorth, 10))
                .Returns(topPlayers).AtMostOnce().Verifiable();
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.ListRecords(Player.RecordType.NetWorth);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["recordType"], Is.TypeOf(typeof(SelectList)), "The recordType field should be a Select object");
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

        [Test]
        public void ViewRecordHistory()
        {
            // Arrange
            Player player = new Player();
            player.PlayerId = 1;
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer)
                .Returns(player).AtMostOnce().Verifiable();
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.ViewRecordHistory();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["recordType"], Is.InstanceOfType(typeof(SelectList)), "The recordType field should be a SelectList of the record types");

            managerMock.Verify();
        }

        [Test]
        public void GetRecordHistory()
        {
            // Arrange
            Player player = new Player();
            player.PlayerId = 1;
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer)
                .Returns(player).AtMostOnce().Verifiable();
            PlayerRecord[] records = new PlayerRecord[]{ new PlayerRecord() };
            managerMock.Expect(m => m.GetPlayerRecords(1))
                .Returns(records).AtMostOnce().Verifiable();
            PlayerRecordController controller = new PlayerRecordController(managerMock.Object);

            // Act
            ActionResult result = controller.GetRecordHistory(Player.RecordType.NetWorth);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(JsonResult)), "Should return a JSON result");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }
    }
}
