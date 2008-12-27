//-----------------------------------------------------------------------
// <copyright file="PlayerController.cs" company="CosmoMonger">
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
    /// This controller deals with all the player related tasks, 
    /// such as creating a new character, editing/viewing profiles.
    /// </summary>
    public class PlayerController : GameController
    {
        /// <summary>
        /// Redirects to the ViewProfile action
        /// </summary>
        /// <returns>
        /// The ViewProfile action
        /// </returns>
        public ActionResult Index()
        {
            return RedirectToAction("ViewProfile");
        }

        /// <summary>
        /// Returns the CreatePlayer View for the user to fill out with their requested player details.
        /// </summary>
        /// <returns>The CreatePlayer view</returns>
        public ActionResult CreatePlayer()
        {
            Race [] races = this.ControllerGame.GetRaces();
            ViewData["Title"] = "Create Player";
            ViewData["raceId"] = new SelectList(races, "RaceId", "Name");
            return View();
        }

        /// <summary>
        /// Creates a player using the inputed player name and race by calling the User.CreatePlayer method. 
        /// Raises an error if another player with the same name already exists.
        /// Redirects to ViewProfile view if player is successfully created, CreatePlayer view otherwise.
        /// </summary>
        /// <param name="name">The name for the new player.</param>
        /// <param name="raceId">The race id for the new player.</param>
        /// <returns>The ViewProfile view if player is created, CreatePlayer view otherwise.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreatePlayer(string name, int raceId)
        {
            Race race = this.ControllerGame.GetRace(raceId);
            if (race == null)
            {
                ModelState.AddModelError("raceId", "Invalid Race selected");
                return CreatePlayer();
            }

            Player newPlayer = null;
            try
            {
                newPlayer = this.ControllerGame.CurrentUser.CreatePlayer(name, race);
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "name")
                {
                    ModelState.AddModelError("name", ex.Message);
                }
                else
                {
                    Logger.Write("Unknown error when User.CreatePlayer was called with name: " + name + " and race: " + race + " Exception Details: " + ex.ToString(), "Controller", 100, 1004, TraceEventType.Error, "ArgumentException in PlayerController.CreatePlayer");
                    ModelState.AddModelError("_FORM", "Unknown error");
                }

                return CreatePlayer();
            }

            return RedirectToAction("ViewProfile", newPlayer.PlayerId);
        }

        /// <summary>
        /// Looks up the profile data for the passed in player id and returns the ViewProfile view.
        /// </summary>
        /// <param name="playerId">The id of the player to view.</param>
        /// <returns>The ViewProfile view with the Player model data.</returns>
        public ActionResult ViewProfile()
        {
            if (this.ControllerGame.CurrentPlayer == null)
            {
                // Go to the create action
                return RedirectToAction("CreatePlayer");
            }
            else
            {
                // Otherwise, show the profile of the current player
                return ViewProfile(this.ControllerGame.CurrentPlayer.PlayerId);
            }
        }

        /// <summary>
        /// Looks up the profile data for the passed in player id and returns the ViewProfile view.
        /// </summary>
        /// <param name="playerId">The id of the player to view.</param>
        /// <returns>The ViewProfile view with the Player model data.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ViewProfile(int playerId)
        {
            ViewData["Title"] = "View Player Profile";
            ViewData["Player"] = this.ControllerGame.GetPlayer(playerId);
            return View();
        }

        /// <summary>
        /// Edit the current users profile
        /// </summary>
        /// <returns>The EditProfile view</returns>
        public ActionResult EditProfile()
        {
            ViewData["Title"] = "Edit Player Profile";
            return View();
        }

        /// <summary>
        /// Saves the edited profile data using the User.UpdateProfile and Player.UpdateProfile methods.
        /// Returns EditProfile view if there is an error in saving the profile data.
        /// If no error was found the ViewProfile view is returned.
        /// </summary>
        /// <param name="playerId">The player id to update.</param>
        /// <param name="characterName">The updated character name.</param>
        /// <returns>
        /// The ViewProfile action if no error was encountered. 
        /// The EditProfile view is returned on errors requiring a change of input.
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditProfile(int playerId, string characterName)
        {
            Player updatePlayer = this.ControllerGame.GetPlayer(playerId);
            try
            {
                updatePlayer.UpdateProfile(characterName);
            }
            catch (ArgumentException ex)
            {
                // Problem with the profile data, back to the drawing board
                if (ex.ParamName == "name")
                {
                    ModelState.AddModelError("_FORM", ex.Message);
                }
                else
                {
                    Logger.Write("Unknown error when Player.UpdateProfile was called with characterName: " + characterName + " Exception Details: " + ex.ToString(), "Controller", 100, 1003, TraceEventType.Error, "ArgumentException in PlayerController.EditProfile");
                    ModelState.AddModelError("_FORM", "Unknown error");
                }

                return View();
            }

            return RedirectToAction("ViewProfile", updatePlayer.PlayerId);
        }
    }
}
