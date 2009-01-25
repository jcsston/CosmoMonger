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
        public void BankDeposit()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();

            // Give the player some starting bank credits
            testPlayer.BankCredits = 2000;
            testPlayer.CashCredits = 1500;

            // Act
            testPlayer.BankDeposit(1000);

            // Assert
            Assert.That(testPlayer.CashCredits, Is.EqualTo(500), "Player should have 1000 fewer cash credits");
            Assert.That(testPlayer.BankCredits, Is.EqualTo(3000), "Player should have 1000 fewer bank credits");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BankDepositNoBank()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();

            // Move the player to a system without a bank
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            CosmoSystem noBankSystem = (from s in db.CosmoSystems
                                        where !s.HasBank
                                        select s).FirstOrDefault();
            Assert.That(noBankSystem, Is.Not.Null, "There should be at least one system in the galaxy without a bank");
            testPlayer.Ship.CosmoSystem = noBankSystem;

            // Act, should throw an exception
            testPlayer.BankDeposit(1000);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BankDepositTooManyCredits()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            testPlayer.CashCredits = 2000;

            // Act, should throw an exception
            testPlayer.BankDeposit(2500);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BankDepositNegativeCredits()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            testPlayer.CashCredits = 1000;
            testPlayer.BankCredits = 1000;

            // Act, should throw an exception
            testPlayer.BankDeposit(-500);
        }

        [Test]
        public void BankWithdraw()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();

            // Give the player some starting bank credits
            testPlayer.BankCredits = 2000;
            testPlayer.CashCredits = 500;

            // Act
            testPlayer.BankWithdraw(1000);

            // Assert
            Assert.That(testPlayer.CashCredits, Is.EqualTo(1500), "Player should have 1000 more cash credits");
            Assert.That(testPlayer.BankCredits, Is.EqualTo(1000), "Player should have 1000 fewer bank credits");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BankWithdrawNoBank()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();

            // Move the player to a system without a bank
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            CosmoSystem noBankSystem = (from s in db.CosmoSystems
                                        where !s.HasBank
                                        select s).FirstOrDefault();
            Assert.That(noBankSystem, Is.Not.Null, "There should be at least one system in the galaxy without a bank");
            testPlayer.Ship.CosmoSystem = noBankSystem;

            // Act, should throw an exception
            testPlayer.BankWithdraw(1000);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BankWithdrawTooManyCredits()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            testPlayer.BankCredits = 2000;

            // Act, should throw an exception
            testPlayer.BankWithdraw(2500);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BankWithdrawNegativeCredits()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            testPlayer.CashCredits = 1000;
            testPlayer.BankCredits = 1000;

            // Act, should throw an exception
            testPlayer.BankWithdraw(-500);
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

        [Test]
        public void UpdatePlaytimeTracked()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            // Set last time played to 1 minute ago
            testPlayer.LastPlayed = DateTime.Now.AddMinutes(-1);

            // Act
            testPlayer.UpdatePlayTime();

            // Assert
            Assert.That(testPlayer.Alive, Is.True, "Player should still be alive with 1 minute of playtime");
            Assert.That(testPlayer.TimePlayed, Is.GreaterThanOrEqualTo(60), "Player should be at least 60 seconds old");
            Assert.That(testPlayer.TimePlayed, Is.LessThanOrEqualTo(65), "Player shouldn't be more than 65 seconds old");
        }

        [Test]
        public void UpdatePlaytimeTimeout()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            // Set last time played to 6 minute ago
            testPlayer.LastPlayed = DateTime.Now.AddMinutes(-6);

            // Act
            testPlayer.UpdatePlayTime();

            // Assert
            Assert.That(testPlayer.Alive, Is.True, "Player should still be alive");
            Assert.That(testPlayer.TimePlayed, Is.EqualTo(0), "Player should still be 0 seconds old");
        }

        [Test]
        public void UpdatePlaytimeExpired()
        {
            // Arrange
            Player testPlayer = this.CreateTestPlayer();
            // Set time played to 7 days
            testPlayer.TimePlayed = 60 * 60 * 24 * 7;

            // Act
            testPlayer.UpdatePlayTime();
            
            // Assert
            Assert.That(testPlayer.Alive, Is.False, "Player should have died with 7 days of playtime");
        }
    }
}
