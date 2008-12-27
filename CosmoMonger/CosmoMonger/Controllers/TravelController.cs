//-----------------------------------------------------------------------
// <copyright file="TravelController.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
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
        public ActionResult Travel()
        {
            ViewData["Title"] = "Travel Map";
            ViewData["Systems"] = this.ControllerGame.CurrentPlayer.Ship.GetInRangeSystems();
            return View("Travel");
        }

        /// <summary>
        /// Travel to the selected targetSystem via the Ship.Travel method, returns the TravelInProgress view.
        /// </summary>
        /// <param name="targetSystem">The target system.</param>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Travel(int targetSystem)
        {
            CosmoSystem targetSystemModel = this.ControllerGame.GetSystem(targetSystem);
            int travelTime = this.ControllerGame.CurrentPlayer.Ship.Travel(targetSystemModel);

            ViewData["Title"] = "Travel in Progress...";
            ViewData["TravelTime"] = travelTime;
            return View("TravelInProgress");
        }

        /// <summary>
        /// Gets a list of all systems via the GameManager.GetSystems and passes the array 
        /// to the Galaxy view to display a galaxy wide map view.
        /// </summary>
        public ActionResult GalaxyMap()
        {
            ViewData["Title"] = "Galaxy Map";
            ViewData["Systems"] = this.ControllerGame.GetSystems();
            return View();
        }
    }
}
