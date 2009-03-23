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
    public class CombatControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void Attack()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.InProgressCombat)
                .Returns<Combat>(null).Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.Attack();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AttackPlayerInCombat()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.InProgressCombat)
                .Returns(new Combat()).Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.Attack();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AttackValidShip()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Ship targetShip = new Ship();
            managerMock.Expect(m => m.GetShip(1))
                .Returns(targetShip).Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.Attack(targetShip))
                .Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.Attack(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AttackInvalidShip()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Ship targetShip = new Ship();
            managerMock.Expect(m => m.GetShip(2))
                .Returns<Ship>(null).Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.Attack(2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void AttackShipInCombat()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Ship targetShip = new Ship();
            managerMock.Expect(m => m.GetShip(2))
                .Returns(targetShip).Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.Attack(targetShip))
                .Throws(new InvalidOperationException())
                .Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.Attack(2);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void CombatStart()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Mock<Combat> combatMock = new Mock<Combat>();
            combatMock.Expect(c => c.Status)
                .Returns(Combat.CombatStatus.Incomplete);
            combatMock.Expect(c => c.AttackerShip.Name)
                .Returns("Attacker");
            combatMock.Expect(c => c.DefenderShip.Name)
                .Returns("Defender");
            managerMock.Expect(m => m.CurrentPlayer.Ship.InProgressCombat)
                .Returns(combatMock.Object)
                .Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.CombatStart();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void CombatStartInvalid()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.InProgressCombat)
                .Returns<Combat>(null)
                .Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.CombatStart();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("CombatNone"), "Should return the CombatNone view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void CombatComplete()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Mock<Combat> combatMock = new Mock<Combat>();
            combatMock.Expect(c => c.Status)
                .Returns(Combat.CombatStatus.ShipDestroyed);
            managerMock.Expect(m => m.GetCombat(1))
                .Returns(combatMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship);
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.CombatComplete(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void CombatCompleteInProgess()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            Mock<Combat> combatMock = new Mock<Combat>();
            combatMock.Expect(c => c.Status)
                .Returns(Combat.CombatStatus.Incomplete);
            managerMock.Expect(m => m.GetCombat(1))
                .Returns(combatMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship);
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.CombatComplete(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No Errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void CombatCompleteInvalid()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetCombat(1))
                .Returns<Combat>(null)
                .Verifiable();
            CombatController controller = new CombatController(managerMock.Object);

            // Act
            ActionResult result = controller.CombatComplete(1);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("CombatNone"), "Should return the CombatNone view");
            Assert.That(controller.ModelState.IsValid, Is.False, "Errors should be returned");
            managerMock.Verify();
        }
    }
}
