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
    public class PlayerControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void CreatePlayer()
        {
            // Arrange
            Race race1 = new Race();
            race1.RaceId = 1;
            race1.Name = "1";
            Race race2 = new Race();
            race2.RaceId = 2;
            race2.Name = "2";
            Race[] races = { race1, race2 };
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetRaces())
                .Returns(races).Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.CreatePlayer();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Races"], Is.SameAs(races), "The Races field should have a reference to the race array");
            Assert.That(controller.ViewData["raceId"], Is.TypeOf(typeof(SelectList)), "The raceId field should be a select list");
            managerMock.Verify();
        }

        [Test]
        public void CreatePlayerPostInvalidRace()
        {
            // Arrange
            Race race1 = new Race();
            race1.RaceId = 1;
            race1.Name = "1";
            Race race2 = new Race();
            race2.RaceId = 2;
            race2.Name = "2";
            Race[] races = { race1, race2 };
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetRaces())
                .Returns(races).Verifiable();
            managerMock.Expect(m => m.GetRace(3))
                .Returns<Race>(null).Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.CreatePlayer("test", 3);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            Assert.That(controller.ModelState["raceId"].Errors, Is.Not.Empty, "Race should be the field causing the error");

            managerMock.Verify();
        }

        [Test]
        public void CreatePlayerPostInvalidName()
        {
            // Arrange
            Race race1 = new Race();
            race1.RaceId = 1;
            race1.Name = "1";
            Race race2 = new Race();
            race2.RaceId = 2;
            race2.Name = "2";
            Race[] races = { race1, race2 };
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(m => m.CreatePlayer("test", race2))
                .Throws(new ArgumentException("Invalid name", "name")).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetRaces())
                .Returns(races).Verifiable();
            managerMock.Expect(m => m.GetRace(2))
                .Returns(race2).Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.CreatePlayer("test", 2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            Assert.That(controller.ModelState["name"].Errors, Is.Not.Empty, "Name should be the field causing the error");

            managerMock.Verify();
        }

        [Test]
        public void CreatePlayerPost()
        {
            // Arrange
            Race race1 = new Race();
            race1.RaceId = 1;
            race1.Name = "1";
            Race race2 = new Race();
            race2.RaceId = 2;
            race2.Name = "2";
            Race[] races = { race1, race2 };
            Mock<User> userMock = new Mock<User>();
            userMock.Expect(m => m.CreatePlayer("test", race2))
                .Returns(new Player()).AtMostOnce().Verifiable();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetRace(2))
                .Returns(race2).Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.CreatePlayer("test", 2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");

            managerMock.Verify();
        }

        [Test]
        public void KillPlayer()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.PlayerId)
                .Returns(1).Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Kill())
                .AtMostOnce().Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);
            
            // Act
            ActionResult result = controller.KillPlayer(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void KillPlayerOtherPlayer()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.PlayerId)
                .Returns(1).Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Kill())
                .AtMostOnce().Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act, should throw an exception
            ActionResult result = controller.KillPlayer(2);
        }

        [Test]
        public void PlayerProfile()
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
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.PlayerProfile();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Player"], Is.SameAs(player), "The Player field should have a reference to the player object");
            managerMock.Verify();
        }

        [Test]
        public void PlayerProfileDirect()
        {
            // Arrange
            Player player = new Player();
            player.PlayerId = 1;
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetPlayer(1))
                .Returns(player).AtMostOnce().Verifiable();
            PlayerController controller = new PlayerController(managerMock.Object);

            // Act
            ActionResult result = controller.PlayerProfile(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            Assert.That(controller.ViewData["Player"], Is.SameAs(player), "The Player field should have a reference to the player object");
            managerMock.Verify();
        }
    }
}
