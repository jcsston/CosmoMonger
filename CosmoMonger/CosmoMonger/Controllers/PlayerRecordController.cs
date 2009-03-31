//-----------------------------------------------------------------------
// <copyright file="PlayerRecordController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using CosmoMonger.Models;
    using System.Text.RegularExpressions;
    using System.Collections;
    using System.Collections.Generic;
    using System.Web;

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
            return this.ListRecords(Player.RecordType.NetWorth);
        }

        /// <summary>
        /// This Action will build a list of high ranking records via the GameManager.GetTopPlayers
        /// method to pass to the ListRecords View.
        /// </summary>
        /// <param name="recordType">Type of the record to order the list by.</param>
        /// <returns>The ListRecords view filled in with the top player record model data</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ListRecords(Player.RecordType recordType)
        {
            // Build the record type selection list
            var recordTypes = from Player.RecordType s in Enum.GetValues(typeof(Player.RecordType))
                              select new { ID = s, Name = Regex.Replace(s.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim() };
            ViewData["recordType"] = new SelectList(recordTypes, "ID", "Name", recordType);
            
            // Format the current record type nicely
            ViewData["SelectedRecordType"] = Regex.Replace(recordType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim();

            // Fetch the records
            ViewData["TopRecords"] = this.ControllerGame.GetTopPlayers(recordType, 10);

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
            ViewData["Player"] = this.ControllerGame.GetPlayer(playerId);
            return View();
        }

        /// <summary>
        /// This Action fetches the records of the passed in player via the GameManager.GetPlayerRecords method
        /// and displays the detailed stats for that player through the ViewRecordHistory view.
        /// </summary>
        /// <returns>The ViewRecord view filled in with the player record history model data</returns>
        public ActionResult ViewRecordHistory()
        {
            if (this.ControllerGame.CurrentPlayer == null)
            {
                // Go to the create action
                return RedirectToAction("CreatePlayer", "Player");
            }
            else
            {
                // Otherwise, show the graph 
                int playerId = this.ControllerGame.CurrentPlayer.PlayerId;
                var recordTypes = from Player.RecordType s in Enum.GetValues(typeof(Player.RecordType))
                                  select new { ID = s, Name = Regex.Replace(s.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim() };
                ViewData["recordType"] = new SelectList(recordTypes, "ID", "Name", Player.RecordType.NetWorth);

                ViewData["RecordHistory"] = this.ControllerGame.GetPlayerRecords(playerId);

                return View();
            }
        }

        /// <summary>
        /// This Action will build a list of high ranking records via the GameManager.GetTopPlayers
        /// method to pass to the ListRecords View.
        /// </summary>
        /// <param name="recordType">Type of the record to order the list by.</param>
        /// <returns>The ListRecords view filled in with the top player record model data</returns>
        public JsonResult GetRecordHistory(Player.RecordType recordType)
        {
            // Fetch the records
            ArrayList dataSet = new ArrayList();

            int playerId = this.ControllerGame.CurrentPlayer.PlayerId;
            PlayerRecord[] recordHistory = this.ControllerGame.GetPlayerRecords(playerId);

            FastDynamicPropertyAccessor.PropertyAccessor prop = new FastDynamicPropertyAccessor.PropertyAccessor(typeof(PlayerRecord), recordType.ToString());
            foreach (PlayerRecord record in recordHistory)
            {
                dataSet.Add(new object[] { record.TimePlayed, prop.Get(record) });
            }

            return Json(dataSet);
            //return Json(new { label = recordType.ToString(), data = dataSet });
        }
    }
}
