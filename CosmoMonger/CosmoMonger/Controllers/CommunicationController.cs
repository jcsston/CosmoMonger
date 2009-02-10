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
        /// Redirects to the Inbox action
        /// </summary>
        /// <returns>A redirection to the Inbox action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("Inbox");
        }

        /// <summary>
        /// This action displays the users messages
        /// </summary>
        /// <returns>The Inbox view</returns>
        public ActionResult Inbox(int? page)
        {
            ViewData["Messages"] = this.ControllerGame.CurrentUser.GetMessages().AsPagination(page ?? 1);

            return View();
        }

        /// <summary>
        /// This action views all sent messages
        /// </summary>
        /// <returns>The Sent view</returns>
        public ActionResult Sent(int? page)
        {
            ViewData["Messages"] = this.ControllerGame.CurrentUser.GetMessagesSent().AsPagination(page ?? 1);

            return View();
        }

        /// <summary>
        /// Composes the specified to user id.
        /// </summary>
        /// <param name="toUserId">Optional to user id.</param>
        /// <returns>The Compose view filled in with the target user id, if any.</returns>
        public ActionResult Compose(int? toUserId)
        {
            ViewData["toUserId"] = new SelectList(this.ControllerGame.CurrentUser.BuddyLists, "FriendId", "Friend.UserName", toUserId); 

            return View();
        }

        /// <summary>
        /// Composes the specified to user id.
        /// </summary>
        /// <param name="toUserId">To user id.</param>
        /// <param name="subject">The subject of the messageg.</param>
        /// <param name="message">The message content.</param>
        /// <returns>A Redirect to the Sent action</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Compose(int toUserId, string subject, string message)
        {
            User toUser = this.ControllerGame.GetUser(toUserId);
            if (toUser != null)
            {
                this.ControllerGame.CurrentUser.SendMessage(toUser, subject, message);
            }

            return RedirectToAction("Sent");
        }

        /// <summary>
        /// This action fetches the message and passes it to the ViewMessage view
        /// </summary>
        /// <param name="messageId">The message id of the message to view.</param>
        /// <returns>The ViewMessage view</returns>
        public ActionResult ViewMessage(int messageId)
        {
            Message message = this.ControllerGame.CurrentUser.GetMessage(messageId);
            if (message != null)
            {
                ViewData["From"] = message.SenderUser.UserName;
                ViewData["To"] = message.RecipientUser.UserName;
                ViewData["Subject"] = message.Subject;
                ViewData["Time"] = message.Time;
                ViewData["Content"] = message.Content;
            }
            else
            {
                ModelState.AddModelError("messageId", "Invalid Message Id");
            }

            return View();
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="messageId">The message id to delete.</param>
        /// <returns>A redirect to the Inbox if successful. The DeleteMessage view otherwise.</returns>
        public ActionResult DeleteMessage(int messageId)
        {
            try
            {
                this.ControllerGame.CurrentUser.DeleteMessage(messageId);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("messageId", ex);
            }

            return RedirectToAction("Inbox");
        }

        public JsonResult UnreadMessages()
        {
            IEnumerable<Message> messages = this.ControllerGame.CurrentUser.GetUnreadMessages();
            
            /*
            List<object> msgs = new List<object>();
            foreach (Message message in messages)
            {
                msgs.Add(new { id = message.MessageId, from = message.SenderUser.UserName, subject = message.Subject });
            }
             * */

            return Json(new { length = messages.Count() });
        }
    }
}
