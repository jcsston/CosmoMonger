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

        public ActionResult Index()
        {
            // Check for non-admin users
            if (!this.ControllerGame.CurrentUser.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult FindUser(string name, int? page)
        {
            // Check for non-admin users
            if (!this.ControllerGame.CurrentUser.Admin)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["Matches"] = this.ControllerGame.FindUser(name ?? "").AsPagination(page ?? 1);
            ViewData["name"] = name;

            return View();
        }

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
                ModelState.AddModelError("userId", "Invalid user id");
            }

            return View();
        }

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
                ModelState.AddModelError("userId", "Invalid user id");
            }

            return View();
        }
    }
}
