//-----------------------------------------------------------------------
// <copyright file="TravelController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Web.Mvc;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// This controller deals with the travel related actions such as 
    /// displaying a galaxy or sector map and traveling to an other system.
    /// </summary>
    public class TravelController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TravelController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public TravelController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public TravelController(GameManager manager)
            : base(manager)
        {
        }

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

                    ViewData["TravelTime"] = travelTime;
                    return View("TravelInProgress");
                }
                catch (InvalidOperationException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    this.AddModelError("_FORM", ex, "");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    this.AddModelError("_FORM", ex, "");
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    this.AddModelError("_FORM", ex, "");
                }
            }
            else
            {
                this.AddModelError("targetSystem", "The target system is invalid", targetSystem);
            }

            // If we got down here, then an error was thrown
            return this.Travel();
        }
    }
}
