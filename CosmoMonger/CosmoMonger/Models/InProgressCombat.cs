//-----------------------------------------------------------------------
// <copyright file="InProgressCombat.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension of the partial LINQ class InProgressCombat.
    /// This class handles the combat conditions and scenerios.
    /// </summary>
    public partial class InProgressCombat
    {
        /// <summary>
        /// This is an list of Goods that can be picked up by the opposing player. 
        /// If the goods are not picked up, the goods are deleted from the system.
        /// </summary>
        public ShipGood[] CargoToPickup;

        /// <summary>
        /// A reference to the current Ship ('Player') turn. 
        /// We reference Ships rather than Players because NPCs are not considered Players, 
        /// but do own Ships and can fight.
        /// </summary>
        private Ship PlayerTurn;

        /// <summary>
        /// Fires the primary weapon at the opposing ship. 
        /// If the opposing ship is destoryed then the non-destoryed cargo and credits are 
        /// picked up and victory is declared. If the opposing ship is player driven then 
        /// the player is cloned on a nearby planet with a bank and given a small ship to get started again.
        /// </summary>
        public void FireWeapon()
        {

        }

        /// <summary>
        /// Gives up the rest of the turn and signals that the current ship is surrendering to the opposing ship.
        /// </summary>
        public void OfferSurrender()
        {

        }

        /// <summary>
        /// Accept the surrender of the opposing ship. 
        /// This gives all the goods and credits aboard the opposing ship to the current ship and ends combat.
        /// </summary>
        public void AcceptSurrender()
        {

        }

        /// <summary>
        /// Jettison all of the ships current cargo. 
        /// This will allow the ship to escape if the opposing ship picks up the cargo.
        /// </summary>
        public void JettisonShipCargo()
        {

        }

        /// <summary>
        /// Pickup cargo jettisoned by opposing ship, this will end combat as the other ship will escape. 
        /// If the cargo is not picked up, it is deleted on the next turn.
        /// </summary>
        public void PickupCargo()
        {

        }

        /// <summary>
        /// Uses the rest of the current turn points to charge the jump drive. 
        /// If the jump drive becomes completely charged, the ship escapes and combat is ended.
        /// </summary>
        public void ChargeJumpdrive()
        {

        }

        /// <summary>
        /// Ends the current ships turn, giving up any turn points left.
        /// </summary>
        public void EndTurn()
        {

        }
    }
}
