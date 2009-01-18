//-----------------------------------------------------------------------
// <copyright file="TravelController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This controller deals with the travel related actions such as 
    /// displaying a galaxy or sector map and traveling to an other system.
    /// </summary>
    public class TravelController : GameController
    {
        /// <summary>
        /// Redirect to the Travel action.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("Travel");
        }

        /// <summary>
        /// Show a view of systems within range via the Ship.GetInRangeSystems method and the Travel view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Travel()
        {
            ViewData["Title"] = "Travel Map";

            // Check if the player is still traveling
            ViewData["IsTraveling"] = this.ControllerGame.CurrentPlayer.Ship.CheckIfTraveling();

            // Fill out ranges
            ViewData["GalaxySize"] = this.ControllerGame.GetGalaxySize();
            ViewData["Range"] = this.ControllerGame.CurrentPlayer.Ship.JumpDrive.Range;

            // Fill system details
            ViewData["CurrentSystem"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem;
            ViewData["InRangeSystems"] = this.ControllerGame.CurrentPlayer.Ship.GetInRangeSystems();
            ViewData["Systems"] = this.ControllerGame.GetSystems();

            return View("Travel");
        }

        /// <summary>
        /// Travel to the selected targetSystem via the Ship.Travel method, returns the TravelInProgress view.
        /// </summary>
        /// <param name="targetSystem">The target system.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Travel(int targetSystem)
        {
            CosmoSystem targetSystemModel = this.ControllerGame.GetSystem(targetSystem);
            if (targetSystemModel != null)
            {
                // Check if the player is still traveling
                ViewData["IsTraveling"] = this.ControllerGame.CurrentPlayer.Ship.CheckIfTraveling();

                try
                {
                    int travelTime = this.ControllerGame.CurrentPlayer.Ship.Travel(targetSystemModel);

                    ViewData["Title"] = "Travel in Progress...";
                    ViewData["TravelTime"] = travelTime;
                    return View("TravelInProgress");
                }
                catch (InvalidOperationException ex)
                {
                    this.ViewData.ModelState.AddModelError("_FORM", ex);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    this.ViewData.ModelState.AddModelError("_FORM", ex);
                }
            }
            else
            {
                this.ViewData.ModelState.AddModelError("_FORM", "The target system is invalid");
            }

            // If we got down here, then an error was thrown
            return this.Travel();
        }
    }
}
