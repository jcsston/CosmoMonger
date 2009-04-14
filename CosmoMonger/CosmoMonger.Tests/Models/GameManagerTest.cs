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
    using MvcContrib.Pagination;

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
        public void GetRace()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Race race = manager.GetRace(1);
            Assert.That(race, Is.Not.Null, "We should get a race");
            Assert.That(race.RaceId, Is.EqualTo(1), "Race should have an id of 1");
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
        public void GetGoods()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Good[] goods = manager.GetGoods();
            Assert.That(goods.Length, Is.GreaterThanOrEqualTo(1), "We need at least one good");

            Good waterGood = (from g in goods
                              where g.Name == "Water"
                              select g).SingleOrDefault();
            Assert.That(waterGood, Is.Not.Null, "The water good should exist");
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
        public void GetShip()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Ship playerShip = manager.GetShip(testPlayer.ShipId);
            Assert.That(playerShip, Is.Not.Null, "We should get a ship");
            Assert.That(playerShip.ShipId, Is.EqualTo(testPlayer.ShipId), "Ship should have the correct id");
        }

        [Test]
        public void GetGalaxySize()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            Assert.That(manager.GetGalaxySize(), Is.GreaterThan(1), "Galaxy should be larger than 1x1");
        }

        [Test]
        public void GetTopPlayersBounty()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            PlayerTopRecord[] topPlayers = null;
            topPlayers = manager.GetTopPlayers(Player.RecordType.Bounty, 20);
            Assert.That(topPlayers.Length, Is.LessThanOrEqualTo(20), "Returned array should be 20 or less items");
        }

        [Test]
        public void GetTopPlayersNetWorth()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            PlayerTopRecord[] topPlayers = manager.GetTopPlayers(Player.RecordType.NetWorth, 10);
            Assert.That(topPlayers, Is.Not.Null, "GetTopPlayers should return an array of records");
            Assert.That(topPlayers.Length, Is.LessThanOrEqualTo(10), "Returned array should be 10 or less items");
        }

        [Test]
        public void GetPlayerRecord()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            PlayerRecord[] records = manager.GetPlayerRecords(testPlayer.PlayerId);
            Assert.That(records, Is.Not.Null, "SHould at least get an array reference for Player records");
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

        [Test]
        [Explicit("Test case for odd search bug #112")]
        public void FindUserJory()
        {
            Player testPlayer = this.CreateTestPlayer();
            GameManager manager = new GameManager(testPlayer.User.UserName);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            
            //db.Log = Console.Out;

            IEnumerable<User> users = manager.FindUser("jory");
            IPagination<User> usersPaged =  users.AsPagination(1);
            usersPaged.ToArray();

            //db.Log = null;

            Assert.That(users, Is.Not.Empty, "Should find me");
        }
    }
}
