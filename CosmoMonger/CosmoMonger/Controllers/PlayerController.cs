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
    /// This controller deals with all the player related tasks, 
    /// such as creating a new character, editing/viewing profiles.
    /// </summary>
    public class PlayerController : GameController
    {
        /// <summary>
        /// Redirects to ViewProfile with the current player id.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            if (this.ControllerGame.CurrentPlayer == null)
            {
                // Go to the create action
                return RedirectToAction("CreatePlayer");
            }
            else
            {
                // Otherwise, show the profile of the current player
                return RedirectToAction("ViewProfile", this.ControllerGame.CurrentPlayer);
            }
        }

        /// <summary>
        /// Returns the CreatePlayer View for the user to fill out with their requested player details.
        /// </summary>
        /// <returns>Create View</returns>
        public ActionResult CreatePlayer()
        {
            return View();
        }

        /// <summary>
        /// Creates a player using the inputed player name and race by calling the User.CreatePlayer method. 
        /// Raises an error if another player with the same name already exists.
        /// Redirects to ViewProfile view if player is successfully created, CreatePlayer view otherwise.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="raceId">The race id.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreatePlayer(string name, int raceId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looks up the profile data for the passed in player id and returns the ViewProfile view.
        /// </summary>
        /// <param name="playerId">The player id.</param>
        public ActionResult ViewProfile(int playerId)
        {
            return View(this.ControllerGame.GetPlayer(playerId));
        }

        /// <summary>
        /// Edit the current users profile
        /// </summary>
        /// <returns>The EditProfile view</returns>
        public ActionResult EditProfile()
        {
            return View();
        }

        /// <summary>
        /// Saves the edited profile data using the User.UpdateProfile and Player.UpdateProfile methods.
        /// Returns EditProfile view if there is an error in saving the profile data. 
        /// If no error was found the ViewProfile view is returned.
        /// </summary>
        /// <param name="playerId">The player id to update.</param>
        /// <param name="characterName">The updated character name.</param>
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
                return RedirectToAction("EditProfile");
            }

            return RedirectToAction("ViewProfile", this.ControllerGame.GetPlayer(playerId));
        }
    }
}
