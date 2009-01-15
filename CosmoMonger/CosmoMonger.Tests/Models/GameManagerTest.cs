namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CosmoMonger.Models;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class GameManagerTest : BasePlayerTest
    {
        [Test]
        public void ConstructorEmptyUsername()
        {
            GameManager manager = null;
            try
            {
                manager = new GameManager("");
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("username"), "Failed argument should be username");
            }
            Assert.That(manager, Is.Null, "An empty username should fail and not create a GameManager");
        }

        [Test]
        public void CurrentUser()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.CurrentUser, Is.EqualTo(testPlayer.User), "GameManager should reference the test user");
        }

        [Test]
        public void CurrentPlayer()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.CurrentPlayer, Is.EqualTo(testPlayer), "GameManager should reference the test player");
        }

        [Test]
        public void GetSystems()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            CosmoSystem [] systems = manager.GetSystems();
            Assert.That(systems.Length, Is.GreaterThanOrEqualTo(1), "At least one system should be in the galaxy");

            CosmoSystem solSystem = (from s in systems
                                     where s.Name == "Sol"
                                     select s).SingleOrDefault();
            Assert.That(solSystem, Is.Not.Null, "Sol system should exist in the galaxy");
        }

        [Test]
        public void GetGalaxySize()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.GetGalaxySize(), Is.GreaterThan(1), "Galaxy should be larger than 1x1");
        }
    }
}
