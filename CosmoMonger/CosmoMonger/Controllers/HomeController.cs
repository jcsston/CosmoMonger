//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
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
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;

    /// <summary>
    /// This controller handles the home pages of CosmoMonger
    /// </summary>
    [CosmoMongerHandleError]
    public class HomeController : Controller
    {
        /// <summary>
        /// Handles the default Index action
        /// </summary>
        /// <returns>The Index View</returns>
        public ActionResult Index()
        {
            ViewData["Title"] = "CosmoMonger";
            //ViewData["Message"] = "Own the Cosmos!";
            ViewData["Message"] = "Buy low, sell high, and destroy your opponents!";
            
            return View();
        }

        /// <summary>
        /// Handles the About action
        /// </summary>
        /// <returns>The About View</returns>
        public ActionResult About()
        {
            ViewData["Title"] = "About CosmoMonger";

            return View();
        }
    }
}
