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
    public class CombatTest : BasePlayerTest
    {
        private Combat combat;
        private Player player1;
        private Player player2;

        [SetUp]
        public void StartCombat()
        {
            this.Cleanup();
            player1 = this.CreateTestPlayer();
            player2 = this.CreateTestPlayer();
            player1.Ship.Attack(player2.Ship);

            combat = player1.Ship.InProgressCombat;

            Assert.That(combat.AttackerShip, Is.EqualTo(player1.Ship), "Player 1 is the attacker");
            Assert.That(combat.DefenderShip, Is.EqualTo(player2.Ship), "Player 2 is the defender");
            Assert.That(combat.ShipTurn, Is.EqualTo(player1.Ship), "It is player 1's turn");
            Assert.That(combat.ShipOther, Is.EqualTo(player2.Ship), "Player 2 is the other ship");
            Assert.That(combat.TurnPointsLeft, Is.EqualTo(Combat.PointsPerTurn), "Turn points left should match points per turn");
        }

        [Test]
        public void FireWeapon()
        {
            combat.FireWeapon();

            Assert.That(combat.ShipTurn, Is.EqualTo(player1.Ship), "Should still be player 1's turn");
            Assert.That(combat.TurnPointsLeft, Is.LessThan(Combat.PointsPerTurn), "Should have fewer turn points left now");
        }

        [Test]
        public void FireWeaponExactlyEnoughTurnPoints()
        {
            // Set the number of turn points left to just enough to fire the weapon
            combat.TurnPointsLeft = player1.Ship.Weapon.TurnCost;

            combat.FireWeapon();

            Assert.That(combat.ShipTurn, Is.EqualTo(player2.Ship), "Should now be player 2's turn");
            Assert.That(combat.TurnPointsLeft, Is.EqualTo(Combat.PointsPerTurn), "Player 2's turn points left should match points per turn");
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException), MatchType = MessageMatch.Contains, ExpectedMessage = "Not enough turn points left")]
        public void FireWeaponNotEnoughTurnPoints()
        {
            // Set the number of turn points left to one less than the number required to fire the weapon
            combat.TurnPointsLeft = player1.Ship.Weapon.TurnCost - 1;

            combat.FireWeapon();
        }

        [Test]
        public void OfferSurrender()
        {
            combat.OfferSurrender();

            Assert.That(combat.ShipTurn, Is.EqualTo(player2.Ship), "Should now be player 2's turn");
            Assert.That(combat.TurnPointsLeft, Is.EqualTo(Combat.PointsPerTurn), "Player 2's turn points left should match points per turn");
            Assert.That(combat.Surrendered, Is.True, "Player 1 should have surrendered");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "already offered surrender")]
        public void OfferSurrenderToSurrenderingShip()
        {
            // Player 1 offers surrender
            combat.OfferSurrender();

            // Now Player 2 tries to surrender also
            // This is not allowed (does not make any sense to do so)
            combat.OfferSurrender();
        }

        [Test]
        public void AcceptSurrenderWithGoods()
        {
            // Add good 1 to player 1
            player1.Ship.AddGood(1, 10);

            combat.OfferSurrender();
            combat.AcceptSurrender();

            ShipGood good1 = player2.Ship.GetGood(1);
            Assert.That(good1.Quantity, Is.EqualTo(10), "Player 2 should have 10 of good 1 now");
        }

        [Test]
        public void AcceptSurrenderNoGoods()
        {
            combat.OfferSurrender();
            combat.AcceptSurrender();

            Assert.That(player2.Ship.GetGoods(), Has.Length(0), "Player 2 should not have picked up any goods");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType=MessageMatch.Contains, ExpectedMessage="No surrender")]
        public void AcceptSurrenderNoSurrender()
        {
            combat.AcceptSurrender();
        }

        [Test]
        public void JettisonCargo()
        {
            // Add good 1 to player 1
            player1.Ship.AddGood(1, 10);

            // Player 1 jettisons cargo
            combat.JettisonCargo();

            Assert.That(combat.ShipTurn, Is.EqualTo(player2.Ship), "Should now be player 2's turn");
            Assert.That(combat.TurnPointsLeft, Is.EqualTo(Combat.PointsPerTurn), "Player 2's turn points left should match points per turn");
            Assert.That(combat.CargoJettisoned, Is.True, "Player 1 should have jettisoned cargo");
            Assert.That(combat.CargoJettisonedCount, Is.EqualTo(10), "10 items should have been jettisoned");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "No")]
        public void JettisonCargoWithNoGoods()
        {
            combat.JettisonCargo();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "already")]
        public void JettisonCargoToJettisonedCargoShip()
        {
            // Add good 1 to player 1
            player1.Ship.AddGood(1, 10);

            // Player 1 jettisons cargo
            combat.JettisonCargo();

            // Add good 1 to player 2
            player2.Ship.AddGood(1, 10);

            // Now Player 2 tries to jettison cargo also
            // This is not allowed (does not make any sense to do so)
            combat.JettisonCargo();
        }

        [Test]
        public void PickupCargo()
        {
            // Add good 1 to player 1
            player1.Ship.AddGood(1, 10);

            combat.JettisonCargo();
            combat.PickupCargo();

            ShipGood good1 = player2.Ship.GetGood(1);
            Assert.That(good1.Quantity, Is.EqualTo(10), "Player 2 should have 10 of good 1 now");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "No")]
        public void PickupCargoNoCargo()
        {
            combat.PickupCargo();
        }

        [Test]
        public void ChargeJumpdrive()
        {
            combat.ChargeJumpDrive();

            Assert.That(combat.ShipTurn, Is.EqualTo(player2.Ship), "Should now be Player 2's turn");
            Assert.That(combat.TurnPointsLeft, Is.EqualTo(Combat.PointsPerTurn), "Player 2's turn points left should match points per turn");
            Assert.That(player1.Ship.CurrentJumpDriveCharge, Is.GreaterThan(0), "Player 1 Jumpdrive should be charged some");
        }

        [Test]
        public void StartSearch()
        {
            combat.StartSearch();

            Assert.That(combat.ShipTurn, Is.EqualTo(player2.Ship), "Should now be player 2's turn");
            Assert.That(combat.TurnPointsLeft, Is.EqualTo(Combat.PointsPerTurn), "Player 2's turn points left should match points per turn");
            Assert.That(combat.Search, Is.True, "Player 1 should have requested a search");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "Already searching")]
        public void StartSearchOfSearchingShip()
        {
            // Player 1 starts searching player 2
            combat.StartSearch();

            // Now Player 2 tries to search also
            // This is not allowed (does not make any sense to do so)
            combat.StartSearch();
        }

        [Test]
        public void AcceptSearchWithGoods()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            Good contrabandGood = (from g in db.Goods
                                   where g.Contraband
                                   select g).FirstOrDefault();
            Good nonContrabandGood = (from g in db.Goods
                                      where !g.Contraband
                                      select g).FirstOrDefault();

            // Add contraband and non-contraband good to player 2
            player2.Ship.AddGood(contrabandGood.GoodId, 5);
            player2.Ship.AddGood(nonContrabandGood.GoodId, 5);

            // Player 1 starts search
            combat.StartSearch();

            // Player 2 accepts search
            combat.AcceptSearch();

            ShipGood good1 = player2.Ship.GetGood(contrabandGood.GoodId);
            Assert.That(good1.Quantity, Is.EqualTo(0), "Player 2 should have 0 of the contraband good now");
            ShipGood good2 = player2.Ship.GetGood(nonContrabandGood.GoodId);
            Assert.That(good2.Quantity, Is.EqualTo(5), "Player 2 should still have 5 of the non-contraband good now");
        }

        [Test]
        public void AcceptSearchNoGoods()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            Good nonContrabandGood = (from g in db.Goods
                                      where !g.Contraband
                                      select g).FirstOrDefault();

            // Add non-contraband good to player 2
            player2.Ship.AddGood(nonContrabandGood.GoodId, 5);

            // Player 1 starts search
            combat.StartSearch();

            // Player 2 accepts search
            combat.AcceptSearch();

            ShipGood good2 = player2.Ship.GetGood(nonContrabandGood.GoodId);
            Assert.That(good2.Quantity, Is.EqualTo(5), "Player 2 should still have 5 of the non-contraband good now");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "No search")]
        public void AcceptSearchWithNoSearch()
        {
            combat.AcceptSearch();
        }

    }
}
