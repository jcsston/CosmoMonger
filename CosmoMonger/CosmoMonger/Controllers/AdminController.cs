namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using CosmoMonger.Models;
    using MvcContrib.Pagination;

    public class AdminController : GameController
    {
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
