using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace CosmoMonger.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
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
