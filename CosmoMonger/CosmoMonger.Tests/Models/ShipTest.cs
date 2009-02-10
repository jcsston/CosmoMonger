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
    /// Summary description for ShipTest
    /// </summary>
    [TestFixture]
    public class ShipTest : BasePlayerTest
    {
        [Test]
        public void GetInRangeSystems()
        {
            Player testPlayer = this.CreateTestPlayer();

            CosmoSystem [] inRangeSystems =  testPlayer.Ship.GetInRangeSystems();

            Assert.That(inRangeSystems, Is.Not.CollectionContaining(testPlayer.Ship.CosmoSystem), "In range systems should not include current system");
        }

        [Test]
        public void CargoSpaceFree()
        {
            // Arrange
            Ship ship = new Ship();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.CargoSpace = 30;

            ship.JumpDrive = new JumpDrive();
            ship.JumpDrive.CargoCost = 1;

            ship.Shield = new Shield();
            ship.Shield.CargoCost = 2;

            ship.Weapon = new Weapon();
            ship.Weapon.CargoCost = 3;

            // Assert
            Assert.That(ship.CargoSpaceFree, Is.EqualTo(30-6), "Total free cargo space should equal the BaseShip amount minus 6");
        }

        [Test]
        public void CargoSpaceTotal()
        {
            // Arrange
            BaseShip baseShip = new BaseShip();
            baseShip.CargoSpace = 30;
            
            Ship ship = new Ship();
            ship.BaseShip = baseShip;

            // Assert
            Assert.That(ship.CargoSpaceTotal, Is.EqualTo(30), "Total cargo space should equal the BaseShip amount");
        }

        [Test]
        public void TradeInValue()
        {
            // Arrange
            Ship ship = new Ship();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.BasePrice = 1000;
            ship.CosmoSystem = new CosmoSystem();

            // Assert
            Assert.That(ship.TradeInValue, Is.EqualTo(800), "Trade in value should equal 80% of the BaseShip value amount");
        }

        [Test]
        public void TradeInValueFromSystem()
        {
            // Arrange
            Ship ship = new Ship();
            ship.BaseShip = new BaseShip();
            ship.BaseShip.BasePrice = 1000;
            ship.CosmoSystem = new CosmoSystem();
            SystemShip systemShip = new SystemShip();
            systemShip.BaseShip = ship.BaseShip;
            systemShip.PriceMultiplier = 2.0;
            ship.CosmoSystem.SystemShips.Add(systemShip);

            // Assert
            Assert.That(ship.TradeInValue, Is.EqualTo(1600), "Trade in value should equal 80% of 2x BaseShip value amount");
        }
    }
}
