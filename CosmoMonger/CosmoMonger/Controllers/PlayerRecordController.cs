//-----------------------------------------------------------------------
// <copyright file="PlayerRecordController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Web.Mvc;
    using CosmoMonger.Models;

    /// <summary>
    /// This controller handles listing the hall of fame player records 
    /// and allowing the user to view their own records.
    /// </summary>
    public class PlayerRecordController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRecordController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public PlayerRecordController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerRecordController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public PlayerRecordController(GameManager manager) 
            : base(manager)
        {
        }

        /// <summary>
        /// Redirects to the ListRecords action.
        /// </summary>
        /// <returns>A redirect to the ListRecords action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("ListRecords");
        }

        /// <summary>
        /// Redirects to the ListRecords(string) with a default record type of NetWorth.
        /// </summary>
        /// <returns>Returns ListRecords(string) with a record type of NetWorth</returns>
        public ActionResult ListRecords()
        {
            return this.ListRecords("NetWorth");
        }

        /// <summary>
        /// This Action will build a list of high ranking records via the GameManager.GetTopPlayers
        /// method to pass to the ListRecords View.
        /// </summary>
        /// <param name="recordType">Type of the record to order the list by.</param>
        /// <returns>The ListRecords view filled in with the top player record model data</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ListRecords(string recordType)
        {
            ViewData["Title"] = "List Top Player Records";
            ViewData["recordType"] = new SelectList(new string[] 
            { 
                "NetWorth",   
                "BountyTotal",
                "HighestBounty",
                "ShipsDestroyed",
                "ForcedSurrenders",
                "ForcedFlees",
                "CargoLooted",
                "ShipsLost",
                "SurrenderCount",
                "FleeCount",
                "CargoLost"
            }, recordType);
            ViewData["TopRecords"] = this.ControllerGame.GetTopPlayers(recordType, 10);
            ViewData["SelectedRecordType"] = recordType;
            return View();
        }

        /// <summary>
        /// This redirects to the ViewRecord(int) Action with the playerId filled in with the current player.
        /// </summary>
        /// <returns>The result of the ViewRecord(int) filled with the current player id</returns>
        public ActionResult ViewRecord()
        {
            if (this.ControllerGame.CurrentPlayer == null)
            {
                // Go to the create action
                return RedirectToAction("CreatePlayer", "Player");
            }
            else
            {
                // Otherwise, show the profile of the current player
                return this.ViewRecord(this.ControllerGame.CurrentPlayer.PlayerId);
            }
        }

        /// <summary>
        /// This Action fetches the passed in player via the GameManager.GetPlayer method
        /// and displays the detailed stats for that player through the ViewRecord view.
        /// </summary>
        /// <param name="playerId">The player id of the player record to display.</param>
        /// <returns>The ViewRecord view filled in with the player model data</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ViewRecord(int playerId)
        {
            ViewData["Title"] = "View Player Record";
            ViewData["Player"] = this.ControllerGame.GetPlayer(playerId);
            return View();
        }
    }
}
