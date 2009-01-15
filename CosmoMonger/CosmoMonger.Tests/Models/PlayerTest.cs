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

    /// <summary>
    /// Summary description for PlayerTest
    /// </summary>
    [TestFixture]
    public class PlayerTest : BasePlayerTest
    {
        [Test]
        public void SkummHomeSystem()
        {
            // Test that the created Skumm player starts in the correct home system
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Race skummRace = (from r in db.Races
                              where r.Name == "Skumm"
                              select r).SingleOrDefault();

            this.CheckPlayerHomeSystem(skummRace);
        }

        [Test]
        public void DecapodianHomeSystem()
        {
            // Test that the created Decapodian player starts in the correct home system
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Race decapodianRace = (from r in db.Races
                              where r.Name == "Decapodian"
                              select r).SingleOrDefault();

            this.CheckPlayerHomeSystem(decapodianRace);
        }

        [Test]
        public void BinariteHomeSystem()
        {
            // Test that the created Binarite player starts in the correct home system
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Race binariteRace = (from r in db.Races
                                   where r.Name == "Binarite"
                                   select r).SingleOrDefault();

            this.CheckPlayerHomeSystem(binariteRace);
        }

        [Test]
        public void SchrodinoidHomeSystem()
        {
            // Test that the created Schrodinoid player starts in the correct home system
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Race schrodinoidRace = (from r in db.Races
                                 where r.Name == "Schrodinoid"
                                 select r).SingleOrDefault();

            this.CheckPlayerHomeSystem(schrodinoidRace);
        }

        [Test]
        public void HumanHomeSystem()
        {
            // Test that the created Human player starts in the correct home system
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Race humanRace = (from r in db.Races
                             where r.Name == "Skumm"
                             select r).SingleOrDefault();

            this.CheckPlayerHomeSystem(humanRace);
        }

        private void CheckPlayerHomeSystem(Race testRace)
        {
            // Test that the created player starts in the correct home system
            Player testPlayer = this.CreateTestPlayer(testRace);
            Assert.AreEqual(testRace.HomeSystem, testPlayer.Ship.CosmoSystem, "Player Ship should start out in the player's home system");
        }

        [Test]
        public void UpdateProfile1()
        {
            // We first need to create a player
            Player testPlayer = this.CreateTestPlayer();
            string playerName = testPlayer.Name;

            // Update player profile
            testPlayer.UpdateProfile("1" + playerName);

            // Check results
            Assert.AreEqual("1" + playerName, testPlayer.Name, "Player name is updated");
        }

        [Test]
        public void UpdateProfile2()
        {
            // We first need to create a player
            Player testPlayer = this.CreateTestPlayer();
            string playerName = testPlayer.Name;

            // Update player profile
            testPlayer.UpdateProfile(playerName);

            // Check results
            Assert.AreEqual(playerName, testPlayer.Name, "Player name is the same");
        }

        [Test]
        public void UpdateProfile3()
        {
            // We first need to create a player
            Player testPlayer = this.CreateTestPlayer();
            string playerName = testPlayer.Name;

            // Update player profile
            testPlayer.UpdateProfile("1" + playerName);

            // Check results
            Assert.AreEqual("1" + playerName, testPlayer.Name, "Player name is updated");

            // Restore player profile
            testPlayer.UpdateProfile(playerName);

            // Check results
            Assert.AreEqual(playerName, testPlayer.Name, "Player name is restored");
        }
    }
}
