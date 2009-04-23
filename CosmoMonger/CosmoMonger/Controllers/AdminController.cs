//-----------------------------------------------------------------------
// <copyright file="AdminController.cs" company="CosmoMonger">
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
    using MvcContrib.Pagination;

    /// <summary>
    /// This controller handles all the admin features
    /// </summary>
    public class AdminController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public AdminController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public AdminController(GameManager manager) 
            : base(manager)
        {
        }

        /// <summary>
        /// Action for the list of admin actions, if the user is not an admin. 
        /// A redirect to the home page is returned.
        /// </summary>
        /// <returns>Index view if user has admin access, redirect to home page if user lacks admin access.</returns>
        public ActionResult Index()
        {
            // Check for non-admin users
            if (!this.ControllerGame.CurrentUser.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Finds a user by name
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <param name="page">The index of the results page to display.</param>
        /// <returns>The FindUser View with the search results.</returns>
        public ActionResult FindUser(string name, int? page)
        {
            // Check for non-admin users
            if (!this.ControllerGame.CurrentUser.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Matches"] = this.ControllerGame.FindUser(name ?? String.Empty).AsPagination(page ?? 1);
            ViewData["name"] = name;

            return View();
        }

        /// <summary>
        /// Bans the user.
        /// </summary>
        /// <param name="userId">The user id to ban.</param>
        /// <returns>The BanUser View, detailing if the user was banned or not.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BanUser(int userId)
        {
            // Check for non-admin users
            if (!this.ControllerGame.CurrentUser.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            User user = this.ControllerGame.GetUser(userId);
            if (user != null)
            {
                ViewData["UserName"] = user.UserName;
                user.Ban();
            }
            else
            {
                ModelState.AddModelError("userId", "Invalid user id", userId);
            }

            return View();
        }

        /// <summary>
        /// Unbans the user.
        /// </summary>
        /// <param name="userId">The user id to unban.</param>
        /// <returns>The UnbanUser View, detailing if the user was unbanned or not.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnbanUser(int userId)
        {
            // Check for non-admin users
            if (!this.ControllerGame.CurrentUser.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            User user = this.ControllerGame.GetUser(userId);
            if (user != null)
            {
                ViewData["UserName"] = user.UserName;
                user.Unban();
            }
            else
            {
                ModelState.AddModelError("userId", "Invalid user id", userId);
            }

            return View();
        }
    }
}
