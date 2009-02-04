//-----------------------------------------------------------------------
// <copyright file="PlayerController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
// <author>Roger Boykin</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Mvc;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This controller deals with all the player related tasks, 
    /// such as creating a new character, editing/viewing profiles.
    /// </summary>
    public class PlayerController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public PlayerController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public PlayerController(GameManager manager) 
            : base(manager)
        {
        }

        /// <summary>
        /// Redirects to the PlayerProfile action
        /// </summary>
        /// <returns>
        /// The PlayerProfile action
        /// </returns>
        public ActionResult Index()
        {
            return RedirectToAction("PlayerProfile");
        }

        /// <summary>
        /// Returns the CreatePlayer View for the user to fill out with their requested player details.
        /// </summary>
        /// <returns>The CreatePlayer view</returns>
        public ActionResult CreatePlayer()
        {
            Race[] races = this.ControllerGame.GetRaces();
            ViewData["Title"] = "Create Player";
            ViewData["raceId"] = new SelectList(races, "RaceId", "Name", ViewData["raceId"]);
            ViewData["Races"] = races;

            return View();
        }

        /// <summary>
        /// Creates a player using the inputed player name and race by calling the User.CreatePlayer method. 
        /// Raises an error if another player with the same name already exists.
        /// Redirects to PlayerProfile view if player is successfully created, CreatePlayer view otherwise.
        /// </summary>
        /// <param name="name">The name for the new player.</param>
        /// <param name="raceId">The race id for the new player.</param>
        /// <returns>The PlayerProfile view if player is created, CreatePlayer view otherwise.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreatePlayer(string name, int raceId)
        {
            Race race = this.ControllerGame.GetRace(raceId);
            if (race == null)
            {
                ModelState.AddModelError("raceId", "Invalid Race selected");
                return this.CreatePlayer();
            }

            Player newPlayer = null;
            try
            {
                newPlayer = this.ControllerGame.CurrentUser.CreatePlayer(name, race);
            }
            catch (ArgumentException ex)
            {
                // Log this exception
                ExceptionPolicy.HandleException(ex, "Controller Policy");

                if (ex.ParamName == "name")
                {
                    ModelState.AddModelError("name", ex.Message);
                }
                else
                {
                    Dictionary<string, object> props = new Dictionary<string, object>
                    { 
                        { "Name", name },
                        { "RaceId", raceId },
                        { "Exception", ex }
                    };
                    Logger.Write("Unknown error when User.CreatePlayer was called", "Controller", 800, 0, TraceEventType.Error, "ArgumentException in PlayerController.CreatePlayer");
                    ModelState.AddModelError("_FORM", "Unknown error");
                }

                // Keep the users selected race
                ViewData["raceId"] = raceId;
                return this.CreatePlayer();
            }

            return RedirectToAction("PlayerProfile", newPlayer.PlayerId);
        }

        /// <summary>
        /// Kills the player.
        /// </summary>
        /// <param name="playerId">The player id.</param>
        /// <returns></returns>
        public ActionResult KillPlayer(int playerId)
        {
            if (this.ControllerGame.CurrentPlayer != null)
            {
                if (this.ControllerGame.CurrentPlayer.PlayerId != playerId)
                {
                    throw new InvalidOperationException("Tried to kill a player other than the current player");
                }

                this.ControllerGame.CurrentPlayer.Kill();
            }

            return RedirectToAction("CreatePlayer");
        }


        /// <summary>
        /// Dead from old age.
        /// </summary>
        /// <returns></returns>
        public ActionResult Dead()
        {


            return View();
        }

        /// <summary>
        /// Looks up the profile data for the current player and returns the PlayerProfile view.
        /// </summary>
        /// <returns>The PlayerProfile view with the current Player model data.</returns>
        public ActionResult PlayerProfile()
        {
            if (this.ControllerGame.CurrentPlayer == null)
            {
                // Go to the create action
                return RedirectToAction("CreatePlayer");
            }
            else
            {
                // Otherwise, show the profile of the current player
                return this.PlayerProfile(this.ControllerGame.CurrentPlayer.PlayerId);
            }
        }

        /// <summary>
        /// Looks up the profile data for the passed in player id and returns the PlayerProfile view.
        /// </summary>
        /// <param name="playerId">The id of the player to view.</param>
        /// <returns>The PlayerProfile view with the Player model data.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PlayerProfile(int playerId)
        {
            ViewData["Title"] = "View Player Profile";
            ViewData["Player"] = this.ControllerGame.GetPlayer(playerId);
            return View();
        }


    }
}
