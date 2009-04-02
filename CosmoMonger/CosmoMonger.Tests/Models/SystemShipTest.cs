namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CosmoMonger.Models;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class SystemShipTest
    {
        private SystemShip CreateSystemShip()
        {
            SystemShip ship = new SystemShip();
            ship.CosmoSystem = new CosmoSystem();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.BasePrice = 10000;
            ship.BaseShip.CargoSpace = 100;
            ship.PriceMultiplier = 0.75;
            ship.Quantity = 1;

            return ship;
        }

        [Test]
        public void Price()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();

            // Act
            int price = ship.Price;

            // Assert
            Assert.That(price, Is.EqualTo(7500), "Price should be 75% of 10000 credits");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType=MessageMatch.Contains, ExpectedMessage="credits")]
        public void BuyNotEnoughCredits()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();
            Mock<User> userMock = new Mock<User>();
            Mock<Ship> shipMock = new Mock<Ship>();
            // Trade value is 500
            shipMock.Expect(s => s.TradeInValue)
                .Returns(500).AtMostOnce().Verifiable();
            // Cash on hand is 5000
            shipMock.Expect(s => s.Credits)
                .Returns(5000).AtMostOnce().Verifiable();

            // Act, should throw an exception
            ship.Buy(shipMock.Object);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), MatchType = MessageMatch.Contains, ExpectedMessage = "cargo space")]
        public void BuyNotEnoughCargoSpace()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();
            Mock<User> userMock = new Mock<User>();
            Mock<Ship> shipMock = new Mock<Ship>();
            // Trade value is 5500
            shipMock.Expect(s => s.TradeInValue)
                .Returns(5500).AtMostOnce().Verifiable();
            // Cargo space is 200, with 50 free
            shipMock.Expect(s => s.CargoSpaceTotal)
                .Returns(200).AtMostOnce().Verifiable();
            shipMock.Expect(s => s.CargoSpaceFree)
                .Returns(50).AtMostOnce().Verifiable();
            // Cash on hand is 5000
            shipMock.Expect(s => s.Credits)
                .Returns(5000).AtMostOnce().Verifiable();

            // Act, should throw an exception
            ship.Buy(shipMock.Object);
        }

        [Test]
        public void Buy()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();

            Mock<Ship> shipMock = new Mock<Ship>();
            // Trade value is 5500
            shipMock.Expect(s => s.TradeInValue)
                .Returns(5500).Verifiable();
            // Cargo space is 50, with 25 free
            shipMock.Expect(s => s.CargoSpaceTotal)
                .Returns(50).Verifiable();
            shipMock.Expect(s => s.CargoSpaceFree)
                .Returns(25).Verifiable();
            // Cash on hand is 5000
            shipMock.Expect(s => s.Credits)
                .Returns(5000).Verifiable();

            // Act
            ship.Buy(shipMock.Object);

            // Assert
            shipMock.Verify();
            // Cost of the ship should be 2000 credits
            shipMock.VerifySet(s => s.Credits, 5000 - 2000);
            Assert.That(ship.Quantity, Is.EqualTo(0), "Should be no ships left in the system of this model");
        }

        [Test]
        public void BuyProfit()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();

            Mock<Ship> shipMock = new Mock<Ship>();
            // Trade value is 15500
            shipMock.Expect(s => s.TradeInValue)
                .Returns(15500).AtMostOnce().Verifiable();
            // Cargo space is 50, with 25 free
            shipMock.Expect(s => s.CargoSpaceTotal)
                .Returns(50).AtMostOnce().Verifiable();
            shipMock.Expect(s => s.CargoSpaceFree)
                .Returns(25).AtMostOnce().Verifiable();
            // Cash on hand is 500
            shipMock.Expect(s => s.Credits)
                .Returns(500).AtMostOnce().Verifiable();

            // Act
            ship.Buy(shipMock.Object);

            // Assert
            shipMock.Verify();
            // Cost of the ship should be -8000 credits
            shipMock.VerifySet(s => s.Credits, 500 - (-8000));
            Assert.That(ship.Quantity, Is.EqualTo(0), "Should be no ships left in the system of this model");
        }

        [Test]
        public void BuyPlayerShipAddedToSystem()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();
            BaseShip playerBaseShip = new BaseShip();

            Mock<Ship> shipMock = new Mock<Ship>();
            // Setup player base ship model
            shipMock.Expect(s => s.BaseShip)
                .Returns(playerBaseShip).Verifiable();
            // Trade value is 5500
            shipMock.Expect(s => s.TradeInValue)
                .Returns(5500).Verifiable();
            // Cargo space is 50, with 25 free
            shipMock.Expect(s => s.CargoSpaceTotal)
                .Returns(50).Verifiable();
            shipMock.Expect(s => s.CargoSpaceFree)
                .Returns(25).Verifiable();
            // Cash on hand is 5000
            shipMock.Expect(s => s.Credits)
                .Returns(5000).Verifiable();

            // Act
            ship.Buy(shipMock.Object);

            // Assert
            shipMock.Verify();
            // Cost of the ship should be 2000 credits
            shipMock.VerifySet(m => m.Credits, 5000 - 2000);
            Assert.That(ship.Quantity, Is.EqualTo(0), "Should be no ships left in the system of this model");
            Assert.That(ship.CosmoSystem.SystemShips.Where(m => m.BaseShip == playerBaseShip && m.Quantity == 1), Is.Not.Empty, "The players base ship should have been added to the system for sale");
        }

        [Test]
        public void BuyPlayerShipAddedToSystem2()
        {
            // Arrange
            SystemShip ship = this.CreateSystemShip();
            BaseShip playerBaseShip = new BaseShip();
            SystemShip playerSystemShip = new SystemShip();
            playerSystemShip.BaseShip = playerBaseShip;
            playerSystemShip.CosmoSystem = ship.CosmoSystem;
            playerSystemShip.Quantity = 2;

            Mock<Ship> shipMock = new Mock<Ship>();
            // Setup player base ship model
            shipMock.Expect(s => s.BaseShip)
                .Returns(playerBaseShip).Verifiable();
            // Trade value is 5500
            shipMock.Expect(s => s.TradeInValue)
                .Returns(5500).Verifiable();
            // Cargo space is 50, with 25 free
            shipMock.Expect(s => s.CargoSpaceTotal)
                .Returns(50).Verifiable();
            shipMock.Expect(s => s.CargoSpaceFree)
                .Returns(25).Verifiable();
            // Cash on hand is 5000
            shipMock.Expect(s => s.Credits)
                .Returns(5000).Verifiable();

            // Act
            ship.Buy(shipMock.Object);

            // Assert
            shipMock.Verify();
            // Cost of the ship should be 2000 credits
            shipMock.VerifySet(m => m.Credits, 5000 - 2000);
            Assert.That(ship.Quantity, Is.EqualTo(0), "Should be no ships left in the system of this model");
            Assert.That(ship.CosmoSystem.SystemShips.Where(m => m.BaseShip == playerBaseShip && m.Quantity == 3), Is.Not.Empty, "The players base ship should have been added to the system for sale");
        }
    }
}
