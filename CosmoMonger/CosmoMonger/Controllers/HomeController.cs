namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;

    [HandleError]
    public class HomeController : Controller
    {
        private GameManager gameManager = null;

        public ActionResult Index()
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                gameManager = new GameManager(User.Identity.Name);
            }
			ViewData["Title"] = "CosmoMonger";
            ViewData["Message"] = "Own the Cosmos!";

            return View();
        }

        public ActionResult About()
        {
			ViewData["Title"] = "About CosmoMonger";

            return View();
        }
    }
}
