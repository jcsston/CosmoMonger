
namespace CosmoMonger.Tests.Models.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CosmoMonger.Models;
    using CosmoMonger.Models.Npcs;
    using NUnit.Framework;
    using Moq;
    using System.Data.Linq;

    [TestFixture]
    public class NpcPoliceTest
    {
        [Test]
        public void DoCombatFireWeapon()
        {
            // Arrange
            Mock<Npc> mockNpcRow = new Mock<Npc>();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CombatId)
                .Returns(2);
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Status)
                .Returns(Combat.CombatStatus.Incomplete);

            // Setup it up so that it's the police turn
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.ShipTurn.ShipId)
                .Returns(1)
                .Verifiable();
            mockNpcRow.Expect(n => n.Ship.ShipId)
                .Returns(1)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Surrendered)
                .Returns(false)
                .Verifiable();
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CargoJettisoned)
                .Returns(false)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.FireWeapon())
                .Returns(true)
                .AtMost(2)
                .Verifiable();

            NpcPolice police = new NpcPolice(mockNpcRow.Object);

            // Act
            police.DoCombat();

            // Assert
            mockNpcRow.Verify();
        }

        [Test]
        public void DoCombatChargeJumpdrive()
        {
            // Arrange
            Mock<Npc> mockNpcRow = new Mock<Npc>();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CombatId)
                .Returns(2);
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Status)
                .Returns(Combat.CombatStatus.Incomplete);

            // Setup it up so that it's the police turn
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.ShipTurn.ShipId)
                .Returns(1)
                .Verifiable();
            mockNpcRow.Expect(n => n.Ship.ShipId)
                .Returns(1)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Surrendered)
                .Returns(false)
                .Verifiable();
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CargoJettisoned)
                .Returns(false)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.FireWeapon())
                .Throws(new ArgumentOutOfRangeException("Out of turn points"))
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.ChargeJumpDrive())
                .Verifiable();

            NpcPolice police = new NpcPolice(mockNpcRow.Object);

            // Act
            police.DoCombat();

            // Assert
            mockNpcRow.Verify();
        }

        [Test]
        public void DoCombatAcceptSurrender()
        {
            // Arrange
            Mock<Npc> mockNpcRow = new Mock<Npc>();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CombatId)
                .Returns(2);
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Status)
                .Returns(Combat.CombatStatus.Incomplete);

            // Setup it up so that it's the police turn
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.ShipTurn.ShipId)
                .Returns(1)
                .Verifiable();
            mockNpcRow.Expect(n => n.Ship.ShipId)
                .Returns(1)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Surrendered)
                .Returns(true)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.AcceptSurrender())
                .Verifiable();

            NpcPolice police = new NpcPolice(mockNpcRow.Object);

            // Act
            police.DoCombat();

            // Assert
            mockNpcRow.Verify();
        }

        [Test]
        public void DoCombatPickupCargo()
        {
            // Arrange
            Mock<Npc> mockNpcRow = new Mock<Npc>();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CombatId)
                .Returns(2);
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Status)
                .Returns(Combat.CombatStatus.Incomplete);

            // Setup it up so that it's the police turn
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.ShipTurn.ShipId)
                .Returns(1)
                .Verifiable();
            mockNpcRow.Expect(n => n.Ship.ShipId)
                .Returns(1)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.Surrendered)
                .Returns(false)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.CargoJettisoned)
                .Returns(true)
                .Verifiable();

            mockNpcRow.Expect(n => n.Ship.InProgressCombat.PickupCargo())
                .Verifiable();

            NpcPolice police = new NpcPolice(mockNpcRow.Object);

            // Act
            police.DoCombat();

            // Assert
            mockNpcRow.Verify();
        }

        [Test]
        public void DoSearch()
        {
            // Arrange

            // This set has a player entry
            EntitySet<Player> playerSet = new EntitySet<Player>();
            playerSet.Add(new Player());

            // This set is empty
            EntitySet<Player> npcSet = new EntitySet<Player>();

            Mock<Ship> playerShip = new Mock<Ship>();
            playerShip.Expect(s => s.Players)
                .Returns(playerSet)
                .Verifiable();
            playerShip.Expect(s => s.ShipId)
                .Returns(1)
                .Verifiable();

            Mock<Ship> lastSearchedShip = new Mock<Ship>();
            lastSearchedShip.Expect(s => s.Players)
                .Returns(playerSet)
                .Verifiable();
            lastSearchedShip.Expect(s => s.ShipId)
                .Returns(2)
                .Verifiable();

            Mock<Ship> playerInCombatShip = new Mock<Ship>();
            playerInCombatShip.Expect(s => s.Players)
                .Returns(playerSet)
                .Verifiable();
            playerInCombatShip.Expect(s => s.InProgressCombat)
                .Returns(new Combat())
                .Verifiable();
            playerInCombatShip.Expect(s => s.ShipId)
                .Returns(3)
                .Verifiable();

            Mock<Ship> npcShip = new Mock<Ship>();
            npcShip.Expect(s => s.Players)
                .Returns(npcSet)
                .Verifiable();
            npcShip.Expect(s => s.ShipId)
                .Returns(4)
                .Verifiable();

            Mock<Npc> mockNpcRow = new Mock<Npc>();
            mockNpcRow.Expect(n => n.LastAttackedShipId)
                .Returns(lastSearchedShip.Object.ShipId);

            mockNpcRow.Expect(n => n.Ship.GetShipsToAttack())
                .Returns(new Ship[] { playerShip.Object, lastSearchedShip.Object, playerInCombatShip.Object, npcShip.Object })
                .Verifiable();

            // The player ship is the only one we can attack/search
            mockNpcRow.Expect(n => n.Ship.Attack(playerShip.Object))
                .AtMostOnce()
                .Verifiable();

            // Police should start searching
            mockNpcRow.Expect(n => n.Ship.InProgressCombat.StartSearch())
                .AtMostOnce()
                .Verifiable();

            NpcPolice police = new NpcPolice(mockNpcRow.Object);

            // Act
            police.DoSearch();

            // Assert
            mockNpcRow.Verify();
        }

        [Test]
        public void DoTravel()
        {
            // Arrange
            Race npcRace = new Race();
            CosmoSystem[] inRangeSystems = { new CosmoSystem(), new CosmoSystem(), new CosmoSystem() };
            inRangeSystems[0].SystemId = 1;
            inRangeSystems[0].Race = new Race();
            inRangeSystems[1].SystemId = 2;
            inRangeSystems[1].Race = npcRace;
            inRangeSystems[2].SystemId = 3;
            inRangeSystems[2].Race = new Race();
            
            Mock<Npc> mockNpcRow = new Mock<Npc>();
            mockNpcRow.Expect(n => n.Ship.GetInRangeSystems())
                .Returns(inRangeSystems)
                .Verifiable();
            mockNpcRow.Expect(n => n.Race)
                .Returns(npcRace)
                .Verifiable();
            // Should try to travel to the system with the same race as the Npc
            mockNpcRow.Expect(n => n.Ship.Travel(inRangeSystems[1]))
                .Returns(6)
                .Verifiable();
            
            NpcPolice police = new NpcPolice(mockNpcRow.Object);

            // Act
            police.DoTravel();

            // Assert
            mockNpcRow.Verify();
        }
    }
}
