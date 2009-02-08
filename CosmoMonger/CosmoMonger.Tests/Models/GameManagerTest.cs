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
        public void GetRaces()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Race[] races = manager.GetRaces();
            Assert.That(races.Length, Is.GreaterThanOrEqualTo(1), "We need at least one race");

            Race humanRace = (from r in races
                                     where r.Name == "Human"
                                     select r).SingleOrDefault();
            Assert.That(humanRace, Is.Not.Null, "The human race should exist");
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

        [Test]
        public void GetTopPlayersEmptyRecordType()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Player[] topPlayers = null;
            try
            {
                topPlayers = manager.GetTopPlayers("", 10);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("recordType"), "recordType should be the failed argument");
            }
            Assert.That(topPlayers, Is.Null, "GetTopPlayers should fail with an empty record type");
        }

        [Test]
        public void GetTopPlayersNetWorth()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Player[] topPlayers = manager.GetTopPlayers("NetWorth", 10);
            Assert.That(topPlayers, Is.Not.Null, "GetTopPlayers should return an array of records");
            Assert.That(topPlayers.Length, Is.LessThanOrEqualTo(10), "Returned array should be 10 or less items");
        }

        [Test]
        public void GetPlayer()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.GetPlayer(testPlayer.PlayerId), Is.EqualTo(testPlayer), "GetPlayer(testPlayerId) should return the test player");
        }

        [Test]
        public void GetUser()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.GetUser(testPlayer.User.UserId), Is.EqualTo(testPlayer.User), "GetUser(testUserId) should return the test user");
        }

        [Test]
        public void GetSystem()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.GetSystem(testPlayer.Ship.SystemId), Is.EqualTo(testPlayer.Ship.CosmoSystem), "GetSystem(systemId) should return the system the test player ship is in");
        }

        [Test]
        public void GetPriceTable()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            List<PriceTableEntry> priceTable = manager.GetPriceTable();
            Assert.That(priceTable, Is.Not.Empty, "GetPriceTable() should return the price table for the galaxy");
        }

        [Test]
        public void FindPlayer()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            IEnumerable<Player> players = manager.FindPlayer(testPlayer.User.UserName);

            Assert.That(players, Is.Not.Empty, "Should at find our own player");
        }

        [Test]
        public void FindUser()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            IEnumerable<User> users = manager.FindUser(testPlayer.User.UserName);

            Assert.That(users, Is.Not.Empty, "Should at find our own user");
        }
    }
}
