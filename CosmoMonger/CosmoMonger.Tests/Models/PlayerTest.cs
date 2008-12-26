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
