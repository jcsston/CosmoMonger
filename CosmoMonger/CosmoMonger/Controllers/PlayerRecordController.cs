namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;

    /// <summary>
    /// This controller handles listing the hall of fame player records 
    /// and allowing the user to view their own records.
    /// </summary>
    public class PlayerRecordController : GameController
    {
        /// <summary>
        /// Redirects to the ListRecords action.
        /// </summary>
        public ActionResult Index()
        {
            return RedirectToAction("ListRecords");
        }

        /// <summary>
	    /// Redirects to the ListRecords(string) with a default record type of NetWorth.
        /// </summary>
        public ActionResult ListRecords()
	    {
            return ListRecords("NetWorth");
	    }

        /// <summary>
	    /// This Action will build a list of high ranking records via the GameManager.GetTopPlayers 
        /// method to pass to the ListRecords View.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ListRecords(string recordType)
	    {
            ViewData["Title"] = "List Top Player Records";
            ViewData["RecordTypes"] = new string [] { 
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
            };
            ViewData["TopRecords"] = this.ControllerGame.GetTopPlayers(recordType, 10);
            return View();
	    }

        /// <summary>
	    /// This redirects to the ViewRecord(int) Action with the playerId filled in with the current player.
        /// </summary>
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
                return ViewRecord(this.ControllerGame.CurrentPlayer.PlayerId);
            }
	    }

        /// <summary>
	    /// This Action fetches the passed in player via the GameManager.GetPlayer method 
        /// and displays the detailed stats for that player through the ViewRecord view.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ViewRecord(int playerId)
	    {
            ViewData["Title"] = "View Player Record";
            ViewData["Player"] = this.ControllerGame.GetPlayer(playerId);
            return View();
	    }
    }
}
