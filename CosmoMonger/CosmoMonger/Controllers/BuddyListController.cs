//-----------------------------------------------------------------------
// <copyright file="BuddyListController.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
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
        /// Initializes a new instance of the <see cref="BuddyListController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public BuddyListController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuddyListController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public BuddyListController(GameManager manager) 
            : base(manager)
        {
        }

        /// <summary>
        /// Redirects to the BuddyList action.
        /// </summary>
        /// <returns>A redirect to the BuddyList action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("BuddyList");
        }

        /// <summary>
        /// Fetch the buddy list for the current player via the User.GetBuddyList method and returns the BuddyList view.
        /// </summary>
        /// <returns>The BuddyList view filled in with the current users buddies</returns>
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
        /// <param name="userId">The user id.</param>
        /// <returns>A redirect to the BuddyList action if successful, The AddBuddy View is returned on error.</returns>
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

                    ModelState.AddModelError("userId", ex.Message, userId);
                }
            }
            else
            {
                ModelState.AddModelError("userId", "Unable to find player", userId);
            }

            return View();
        }

        /// <summary>
        /// Removes the passed in player to the current players buddy list via the User.RemoveBuddy method,
        /// this action is called from the BuddyList view and redirects back to the BuddyList action.
        /// </summary>
        /// <param name="buddyId">The buddy id.</param>
        /// <returns>
        /// A redirect to the BuddyList action if successful, The RemoveBuddy View is returned on error.
        /// </returns>
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

                    ModelState.AddModelError("buddyId", ex.Message, buddyId);
                }
            }
            else
            {
                ModelState.AddModelError("buddyId", "Unable to find player", buddyId);
            }

            return View();
        }

        /// <summary>
        /// Fetch the ignore list for the current player via the User.GetIgnoreList method and returns the IgnoreList view.
        /// </summary>
        /// <returns>The IgnoreList view filled in with the current users Ignore List</returns>
        public ActionResult IgnoreList()
        {
            ViewData["IgnoreList"] = this.ControllerGame.CurrentUser.GetIgnoreList();

            return View();
        }

        /// <summary>
        /// Adds the passed in player to the current players ignore list via the User.AddIgnore method,
        /// this action is called from the IgnoreList view and redirects back to the IgnoreList action.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>A redirect to the IgnoreList action if successful, The AddIgnore View is returned on error.</returns>
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

                    ModelState.AddModelError("userId", ex.Message, userId);
                }
            }
            else
            {
                ModelState.AddModelError("userId", "Unable to find player", userId);
            }

            return View();
        }

        /// <summary>
        /// Removes the passed in player to the current players ignore list via the User.RemoveIgnore method,
        /// this action is called from the IgnoreList view and redirects back to the IgnoreList action.
        /// </summary>
        /// <param name="antiBuddyId">The anti buddy id.</param>
        /// <returns>A redirect to the IgnoreList action if successful, The RemoveIgnore View is returned on error.</returns>
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

                    ModelState.AddModelError("antiBuddyId", ex.Message, antiBuddyId);
                }
            }
            else
            {
                ModelState.AddModelError("antiBuddyId", "Unable to find player", antiBuddyId);
            }

            return View();
        }
    }
}
