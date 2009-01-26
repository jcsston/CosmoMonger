//-----------------------------------------------------------------------
// <copyright file="ShipController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;

    /// <summary>
    /// This controller deals with all ship related tasks such as, 
    /// * Buying a ship
    /// * Buying ship upgrades
    /// * Selling ship upgrades, 
    /// * Viewing current ship information
    /// </summary>
    public class ShipController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShipController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public ShipController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public ShipController(GameManager manager) 
            : base(manager)
        {
        }

        /// <summary>
        /// Redirects to the the ViewShip action.
        /// </summary>
        /// <returns>A redirect to the ViewShip action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("ViewShip");
        }

        /// <summary>
        /// Gets the current players ship and pass it to the ViewShip view.
        /// </summary>
        /// <returns>The ViewShip view filled in with the model data for the players current ship</returns>
        public ActionResult ViewShip()
        {
            ViewData["Title"] = "Current Ship";
            ViewData["Ship"] = this.ControllerGame.CurrentPlayer.Ship;
            return View();
        }

        /// <summary>
        /// This action will fetch the avaiable ships in the current players system 
        /// via the System.GetAvailableShips method and pass the data to the BuyShip view.
        /// </summary>
        /// <returns>The ListShips view filled with the ships available for purchase</returns>
        public ActionResult ListShips()
        {
            ViewData["Title"] = "Available Ships";
            ViewData["Ships"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.GetAvailableShips();
            return View();
        }

        /// <summary>
        /// This action takes in the passed systemShip id and buys the ship via the SystemShip.Buy method. 
        /// Then redirects to the Index action.
        /// </summary>
        /// <param name="shipId">The base ship id of the SystemShip to buy.</param>
        /// <returns>Redirect back to the ViewShip action if successful, returns the ListShips view on error</returns>
        public ActionResult BuyShip(int shipId)
        {
            SystemShip shipToBuy = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.GetShip(shipId);
            if (shipToBuy != null)
            {
                try
                {
                    shipToBuy.Buy(this.ControllerGame);
                    // Success, redirect to display the newly bought ship
                    return RedirectToAction("ViewShip");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("_FORM", ex);
                }
            }
            else
            {
                ModelState.AddModelError("shipId", "Ship is no longer for sell in system");
            }

            // If we got down here, then an error was encountered
            // Get back to the list of ships
            return this.ListShips();
        }

        /* TODO: Implement following actions in RC4 */
        
        /// <summary>
        /// This action will fetch the available ship engine upgrades via the System.GetEngineUpgrades method
        /// and pass the data to the BuyEngineUpgrade view.
        /// </summary>
        /// <returns>Throws NotImplementedException</returns>
        public ActionResult BuyEngineUpgrade()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This action takes in the passed upgrade id,  buys the ship upgrade via the SystemEngineUpgrade.Buy method 
        /// and then redirects to the Index action.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns>Throws NotImplementedException</returns>
        public ActionResult BuyEngineUpgrade(int upgradeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This action will fetch the available ship shield upgrades via the System.GetShieldUpgrades method 
        /// and pass the data to the BuyShieldUpgrade view.
        /// </summary>
        /// <returns>Throws NotImplementedException</returns>
        public ActionResult BuyShieldUpgrade()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This action takes in the passed upgrade id,  buys the ship upgrade via the SystemShieldUpgrade.Buy method, 
        /// and then redirects to the Index action.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns>Throws NotImplementedException</returns>
        public ActionResult BuyShieldUpgrade(int upgradeId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This action will fetch the available ship weapon upgrades via the System.GetWeaponUpgrades method 
        /// and pass the data to the BuyWeaponUpgrade view.
        /// </summary>
        /// <returns>Throws NotImplementedException</returns>
        public ActionResult BuyWeaponUpgrade()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This action takes in the passed upgrade id,  buys the ship upgrade via the SystemWeaponUpgrade.Buy method, 
        /// and then redirects to the Index action.
        /// </summary>
        /// <param name="upgradeId">The upgrade id.</param>
        /// <returns>Throws NotImplementedException</returns>
        public ActionResult BuyWeaponUpgrade(int upgradeId)
        {
            throw new NotImplementedException();
        }
    }
}
