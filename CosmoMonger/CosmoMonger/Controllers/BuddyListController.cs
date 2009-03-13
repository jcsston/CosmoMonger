namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using MvcContrib.Pagination;

    /// <summary>
    /// This controller deals with all the Buddy List managment.
    /// </summary>
    public class BuddyListController : GameController
    {
        /// <summary>
        /// Redirects to the BuddyList action.
        /// </summary>
        public ActionResult Index()
        {
            return RedirectToAction("BuddyList");
        }

        /// <summary>
        /// Fetch the buddy list for the current player via the User.GetBuddyList method and returns the BuddyList view.
        /// </summary>
        public ActionResult BuddyList()
        {
            ViewData["BuddyList"] = this.ControllerGame.CurrentUser.GetBuddyList();

            return View();
        }

        /// <summary>
        /// Finds an active player by name or username.
        /// </summary>
        /// <param name="name">The name to search by.</param>
        /// <param name="page">The page of the results to display.</param>
        /// <returns>
        /// The FindUser view filled in with matches
        /// </returns>
        public ActionResult FindPlayer(string name, int? page)
        {
            ViewData["Matches"] = this.ControllerGame.FindPlayer(name).AsPagination(page ?? 1);
            ViewData["name"] = name;

            return View();
        }

        /// <summary>
        /// Adds the passed in player to the current players buddy list via the User.AddBuddy method, 
        /// this action is called from the BuddyList view and redirects back to the BuddyList action.
        /// </summary>
        /// <param name="userId"></param>
        public ActionResult AddBuddy(int userId)
        {
            User newBuddy = this.ControllerGame.GetUser(userId);
            if (newBuddy != null)
            {
                try
                {
                    this.ControllerGame.CurrentUser.AddBuddy(newBuddy);
                    return RedirectToAction("BuddyList");
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("userId", ex);
                }
            }
            else
            {
                ModelState.AddModelError("userId", "Unable to find player");
            }

            return View();
        }

        /// <summary>
        /// Removes the passed in player to the current players buddy list via the User.RemoveBuddy method, 
        /// this action is called from the BuddyList view and redirects back to the BuddyList action.
        /// </summary>
        /// <param name="buddyId"></param>
        public ActionResult RemoveBuddy(int buddyId)
        {
            User buddy = this.ControllerGame.GetUser(buddyId);
            if (buddy != null)
            {
                try
                {
                    this.ControllerGame.CurrentUser.RemoveBuddy(buddy);
                    return RedirectToAction("BuddyList");
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("buddyId", ex);
                }
            }

            return View();
        }

        /// <summary>
        /// Fetch the ignore list for the current player via the User.GetIgnoreList method and returns the IgnoreList view.
        /// </summary>
        public ActionResult IgnoreList()
        {
            ViewData["IgnoreList"] = this.ControllerGame.CurrentUser.GetIgnoreList();

            return View();
        }

        /// <summary>
        /// Adds the passed in player to the current players ignore list via the User.AddIgnore method, 
        /// this action is called from the IgnoreList view and redirects back to the IgnoreList action.
        /// </summary>
        /// <param name="antiBuddyId"></param>
        public ActionResult AddIgnore(int userId)
        {
            User player = this.ControllerGame.GetUser(userId);
            if (player != null)
            {
                try
                {
                    this.ControllerGame.CurrentUser.AddIgnore(player);
                    return RedirectToAction("IgnoreList");
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("userId", ex);
                }
            }

            return View();
        }

        /// <summary>
        /// Removes the passed in player to the current players ignore list via the User.RemoveIgnore method, 
        /// this action is called from the IgnoreList view and redirects back to the IgnoreList action.
        /// </summary>
        /// <param name="antiBuddyId"></param>
        public ActionResult RemoveIgnore(int antiBuddyId)
        {
            User antiBuddy = this.ControllerGame.GetUser(antiBuddyId);
            if (antiBuddy != null)
            {
                try
                {
                    this.ControllerGame.CurrentUser.RemoveIgnore(antiBuddy);
                    return RedirectToAction("IgnoreList");
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("antiBuddyId", ex);
                }
            }

            return View();
        }
    }
}
