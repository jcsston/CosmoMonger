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
    using System.Collections;

    /// <summary>
    /// This controller handles communication between players
    /// </summary>
    public class CommunicationController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public CommunicationController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public CommunicationController(GameManager manager) 
            : base(manager)
        {
        }

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
        /// <param name="sent">A flag signaling if the message to view was a sent message.</param>
        /// <returns>The ViewMessage view</returns>
        public ActionResult ViewMessage(int messageId, bool? sent)
        {
            Message message = this.ControllerGame.CurrentUser.GetMessage(messageId);
            if (message != null)
            {
                // If these message is for the user, mark it read
                if (message.RecipientUser == this.ControllerGame.CurrentUser)
                {
                    message.MarkAsReceived();
                }

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
            ViewData["Sent"] = sent ?? false;

            return View();
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="messageId">The message id to delete.</param>
        /// <param name="sent">A flag signaling if the message to delete was a sent message.</param>
        /// <returns>
        /// A redirect to the Inbox if successful. The DeleteMessage view otherwise.
        /// </returns>
        public ActionResult DeleteMessage(int messageId, bool? sent)
        {
            try
            {
                this.ControllerGame.CurrentUser.DeleteMessage(messageId);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("messageId", ex);
            }

            // If sent is null, default to false
            if (sent ?? false)
            {
                return RedirectToAction("Sent");
            }
            else
            {
                return RedirectToAction("Inbox");
                
            }
        }

        /// <summary>
        /// Checks if there are any unread messages.
        /// </summary>
        /// <returns>A JSON result containing the number of unread messages.</returns>
        public JsonResult UnreadMessages()
        {
            IEnumerable<Message> messages = this.ControllerGame.CurrentUser.GetUnreadMessages();
            ArrayList messageData = new ArrayList();
            foreach (Message msg in messages)
            {
                messageData.Add(new {
                    from = HttpUtility.HtmlEncode(msg.SenderUser.UserName),
                    time = msg.Time.ToString("R"),
                    subject = HttpUtility.HtmlEncode(msg.Subject),
                    id = msg.MessageId
                });
            }

            return Json(messageData);
        }
    }
}
