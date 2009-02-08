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
    /// This controller handles chatting between players
    /// </summary>
    public class ChatController : GameController
    {
        /// <summary>
        /// Redirects to the Chat action
        /// </summary>
        /// <returns>A redirection to the Chat action</returns>
        public ActionResult Index()
        {
            BuddyList[] buddies = this.ControllerGame.CurrentUser.GetBuddyList();
            ViewData["BuddyList"] = buddies;
            ViewData["friendId"] = new SelectList(buddies, "FriendId", "Friend.Username");

            return View();
        }

        /// <summary>
        /// Chats this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Chat(int friendId)
        {
            ViewData["friendId"] = friendId;

            return View();
        }

        /// <summary>
        /// This action fetches any unread messages via the User.GetUnreadMessages method and 
        /// returns the data in JSON format
        /// </summary>
        /// <returns></returns>
        public JsonResult FetchMessages()
        {
            Message [] messages = this.ControllerGame.CurrentUser.FetchUnreadMessages();

            return Json(messages);
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
