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
    public class TravelControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            TravelController controller = new TravelController(managerMock.Object);

            // Act
            ActionResult result = controller.Index();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(RedirectToRouteResult)), "Should return a redirect");
            Assert.That(controller.ModelState.IsValid, "No errors should be returned");
            managerMock.Verify();
        }

        [Test]
        public void Travel()
        {
            // Arrange         
            Mock<CosmoSystem> currentSystemMock = new Mock<CosmoSystem>();
            
            Mock<CosmoSystem> inRangeSystemMock = new Mock<CosmoSystem>();
            CosmoSystem[] inRangeSystems = new CosmoSystem[] { inRangeSystemMock.Object };
            
            Mock<CosmoSystem> outOfRangeSystemMock = new Mock<CosmoSystem>();
            CosmoSystem[] allSystems = new CosmoSystem[] { currentSystemMock.Object, inRangeSystemMock.Object, outOfRangeSystemMock.Object };

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.CurrentPlayer.Ship.CheckIfTraveling())
                .Returns(false)
                .Verifiable();
            managerMock.Expect(m => m.GetGalaxySize())
                .Returns(30)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.JumpDrive.Range)
                .Returns(12)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem)
                .Returns(currentSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetInRangeSystems())
                .Returns(inRangeSystems)
                .Verifiable();
            managerMock.Expect(m => m.GetSystems())
                .Returns(allSystems)
                .Verifiable();
            TravelController controller = new TravelController(managerMock.Object);

            // Act
            ActionResult result = controller.Travel();

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            Assert.That(controller.ModelState.IsValid, "No error should be returned");
            Assert.That(controller.ViewData["IsTraveling"], Is.False, "The IsTraveling field should be false");
            Assert.That(controller.ViewData["GalaxySize"], Is.EqualTo(30), "The GalaxySize field should be 30");
            Assert.That(controller.ViewData["Range"], Is.EqualTo(12), "The Range field should be 12");
            Assert.That(controller.ViewData["CurrentSystem"], Is.SameAs(currentSystemMock.Object), "The CurrentSystem field should have a reference to the current system object");
            Assert.That(controller.ViewData["InRangeSystems"], Is.EquivalentTo(inRangeSystems), "The InRangeSystems field should have an array containing all the in range systems");
            Assert.That(controller.ViewData["Systems"], Is.EquivalentTo(allSystems), "The Systems field should have an array containing all the systems in the galaxy");

            managerMock.Verify();
        }

        [Test]
        public void TravelInRangeSystem()
        {
            // Arrange         
            Mock<CosmoSystem> currentSystemMock = new Mock<CosmoSystem>();
            int currentSystemId = 1;
            currentSystemMock.Expect(m => m.SystemId)
                .Returns(currentSystemId);

            Mock<CosmoSystem> targetSystemMock = new Mock<CosmoSystem>();
            int targetSystemId = 2;
            targetSystemMock.Expect(m => m.SystemId)
                .Returns(targetSystemId);

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetSystem(targetSystemId))
                .Returns(targetSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CheckIfTraveling())
                .Returns(false)
                .Verifiable();
            int travelTime = 5;
            managerMock.Expect(m => m.CurrentPlayer.Ship.Travel(targetSystemMock.Object))
                .Returns(travelTime)
                .AtMostOnce()
                .Verifiable();
            TravelController controller = new TravelController(managerMock.Object);

            // Act
            ActionResult result = controller.Travel(targetSystemId);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("TravelInProgress"), "Should return the TravelInProgressView");
            Assert.That(controller.ModelState.IsValid, "No error should be returned");
            Assert.That(controller.ViewData["IsTraveling"], Is.False, "The IsTraveling field should be false");
            Assert.That(controller.ViewData["TravelTime"], Is.EqualTo(travelTime), "The TravelTime field should be 5");

            managerMock.Verify();
        }

        [Test]
        public void TravelOutOfRangeSystem()
        {
            // Arrange         
            Mock<CosmoSystem> currentSystemMock = new Mock<CosmoSystem>();
            int currentSystemId = 1;
            currentSystemMock.Expect(m => m.SystemId)
                .Returns(currentSystemId);

            Mock<CosmoSystem> targetSystemMock = new Mock<CosmoSystem>();
            int targetSystemId = 2;
            targetSystemMock.Expect(m => m.SystemId)
                .Returns(targetSystemId);

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetSystem(targetSystemId))
                .Returns(targetSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CheckIfTraveling())
                .Returns(false)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.Travel(targetSystemMock.Object))
                .Throws<ArgumentOutOfRangeException>()
                .AtMostOnce()
                .Verifiable();
            // Normal Travel mocks
            managerMock.Expect(m => m.GetGalaxySize())
                .Returns(30)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.JumpDrive.Range)
                .Returns(12)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem)
                .Returns(currentSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetInRangeSystems())
                .Returns(new CosmoSystem[0])
                .Verifiable();
            managerMock.Expect(m => m.GetSystems())
                .Returns(new CosmoSystem[0])
                .Verifiable();
            TravelController controller = new TravelController(managerMock.Object);

            // Act
            ActionResult result = controller.Travel(targetSystemId);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Travel"), "Should return the Travel View");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["_FORM"].Errors[0].Exception, Is.TypeOf(typeof(ArgumentOutOfRangeException)), "Error for form should be ArgumentOutOfRangeException");
            Assert.That(controller.ViewData["IsTraveling"], Is.False, "The IsTraveling field should be false");

            managerMock.Verify();
        }

        [Test]
        public void TravelWhenTraveling()
        {
            // Arrange         
            Mock<CosmoSystem> currentSystemMock = new Mock<CosmoSystem>();
            int currentSystemId = 1;
            currentSystemMock.Expect(m => m.SystemId)
                .Returns(currentSystemId);

            Mock<CosmoSystem> targetSystemMock = new Mock<CosmoSystem>();
            int targetSystemId = 2;
            targetSystemMock.Expect(m => m.SystemId)
                .Returns(targetSystemId);

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetSystem(targetSystemId))
                .Returns(targetSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CheckIfTraveling())
                .Returns(true)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.Travel(targetSystemMock.Object))
                .Throws<InvalidOperationException>()
                .AtMostOnce()
                .Verifiable();
            // Normal Travel mocks
            managerMock.Expect(m => m.GetGalaxySize())
                .Returns(30)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.JumpDrive.Range)
                .Returns(12)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem)
                .Returns(currentSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetInRangeSystems())
                .Returns(new CosmoSystem[0])
                .Verifiable();
            managerMock.Expect(m => m.GetSystems())
                .Returns(new CosmoSystem[0])
                .Verifiable();
            TravelController controller = new TravelController(managerMock.Object);

            // Act
            ActionResult result = controller.Travel(targetSystemId);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Travel"), "Should return the Travel View");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["_FORM"].Errors[0].Exception, Is.TypeOf(typeof(InvalidOperationException)), "Error for form should be InvalidOperationException");
            Assert.That(controller.ViewData["IsTraveling"], Is.True, "The IsTraveling field should be true");

            managerMock.Verify();
        }

        [Test]
        public void TravelToCurrentSystem()
        {
            // Arrange         
            Mock<CosmoSystem> currentSystemMock = new Mock<CosmoSystem>();
            int currentSystemId = 1;
            currentSystemMock.Expect(m => m.SystemId)
                .Returns(currentSystemId);

            Mock<User> userMock = new Mock<User>();
            Mock<GameManager> managerMock = new Mock<GameManager>(userMock.Object);
            managerMock.Expect(m => m.GetSystem(currentSystemId))
                .Returns(currentSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CheckIfTraveling())
                .Returns(false)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.Travel(currentSystemMock.Object))
                .Throws<ArgumentException>()
                .AtMostOnce()
                .Verifiable();
            // Normal Travel mocks
            managerMock.Expect(m => m.GetGalaxySize())
                .Returns(30)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.JumpDrive.Range)
                .Returns(12)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.CosmoSystem)
                .Returns(currentSystemMock.Object)
                .Verifiable();
            managerMock.Expect(m => m.CurrentPlayer.Ship.GetInRangeSystems())
                .Returns(new CosmoSystem[0])
                .Verifiable();
            managerMock.Expect(m => m.GetSystems())
                .Returns(new CosmoSystem[0])
                .Verifiable();
            TravelController controller = new TravelController(managerMock.Object);

            // Act
            ActionResult result = controller.Travel(currentSystemId);

            // Assert
            Assert.That(result, Is.TypeOf(typeof(ViewResult)), "Should return a view");
            ViewResult viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Travel"), "Should return the Travel View");
            Assert.That(controller.ModelState.IsValid, Is.False, "An error should be returned");
            Assert.That(controller.ModelState["_FORM"].Errors[0].Exception, Is.TypeOf(typeof(ArgumentException)), "Error for form should be ArgumentException");
            Assert.That(controller.ViewData["IsTraveling"], Is.False, "The IsTraveling field should be false");

            managerMock.Verify();
        }
    }
}
