namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;
    using MvcContrib.Pagination;
    using MvcContrib.EnumerableExtensions;

    /// <summary>
    /// This controller handles communication between players
    /// </summary>
    public class CommunicationController : GameController
    {
        /// <summary>
        /// Redirects to the Chat action
        /// </summary>
        /// <returns>A redirection to the Chat action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("Inbox");
        }

        /// <summary>
        /// This action displays the users messages
        /// </summary>
        /// <returns></returns>
        public ActionResult Inbox(int? page)
        {
            ViewData["Messages"] = this.ControllerGame.CurrentUser.Messages.OrderBy(m => m.Time).AsPagination(page ?? 1);

            return View();
        }

        /// <summary>
        /// This action views all sent messages
        /// </summary>
        /// <returns></returns>
        public ActionResult Sent(int? page)
        {
            ViewData["Messages"] = this.ControllerGame.CurrentUser.MessagesSent.OrderBy(m => m.Time).AsPagination(page ?? 1);

            return View();
        }

        public ActionResult Compose()
        {
            ViewData["toUserId"] = new SelectList(this.ControllerGame.CurrentUser.BuddyLists, "FriendId", "Friend.UserName"); 

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Compose(int toUserId, string message)
        {
            User toUser = this.ControllerGame.GetUser(toUserId);
            if (toUser != null)
            {
                this.ControllerGame.CurrentUser.SendMessage(toUser, message);
            }

            return RedirectToAction("Sent");
        }

        /// <summary>
        /// This action sends the passed in message to the target user via the User.SendMessage method 
        /// and returns a true or false flag in JSON format
        /// </summary>
        /// <param name="toUserId">To user id.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public JsonResult SendMessage(int toUserId, string message)
        {
            User toUser = this.ControllerGame.GetUser(toUserId);
            if (toUser != null)
            {
                this.ControllerGame.CurrentUser.SendMessage(toUser, message);
                return Json(true);
            }
            return Json(false);
        }
    }
}
