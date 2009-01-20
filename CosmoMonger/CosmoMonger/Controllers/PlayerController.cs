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
            ViewData["raceId"] = new SelectList(races, "RaceId", "Name");
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
                if (ex.ParamName == "name")
                {
                    ModelState.AddModelError("name", ex.Message);
                }
                else
                {
                    Logger.Write("Unknown error when User.CreatePlayer was called with name: " + name + " and race: " + race + " Exception Details: " + ex.ToString(), "Controller", 100, 1004, TraceEventType.Error, "ArgumentException in PlayerController.CreatePlayer");
                    ModelState.AddModelError("_FORM", "Unknown error");
                }

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
            if (this.ControllerGame.CurrentPlayer.PlayerId != playerId)
            {
                throw new InvalidOperationException("Tried to kill a player other than the current player");
            }

            this.ControllerGame.CurrentPlayer.Kill();

            return RedirectToAction("CreatePlayer");
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

        /// <summary>
        /// Edit the current users profile
        /// </summary>
        /// <returns>The UserProfile view</returns>
        public ActionResult UserProfile()
        {
            ViewData["Title"] = "Edit Player Profile";
            return View();
        }

        /// <summary>
        /// Saves the edited profile data using the User.UpdateProfile and Player.UpdateProfile methods.
        /// Returns UserProfile view if there is an error in saving the profile data.
        /// If no error was found the PlayerProfile view is returned.
        /// </summary>
        /// <param name="playerId">The player id to update.</param>
        /// <param name="characterName">The updated character name.</param>
        /// <returns>
        /// The PlayerProfile action if no error was encountered. 
        /// The UserProfile view is returned on errors requiring a change of input.
        /// </returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserProfile(int playerId, string characterName)
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
                    Logger.Write("Unknown error when Player.UpdateProfile was called with characterName: " + characterName + " Exception Details: " + ex.ToString(), "Controller", 100, 1003, TraceEventType.Error, "ArgumentException in PlayerController.UserProfile");
                    ModelState.AddModelError("_FORM", "Unknown error");
                }

                return View();
            }

            return RedirectToAction("PlayerProfile", updatePlayer.PlayerId);
        }
    }
}
