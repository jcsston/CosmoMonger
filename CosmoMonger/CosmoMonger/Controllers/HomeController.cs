//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Threading;
    using System.Web.Mvc;
    using CosmoMonger.Controllers.Attributes;
    using CosmoMonger.Models;

    /// <summary>
    /// This controller handles the home pages of CosmoMonger
    /// </summary>
    [ExceptionPolicy]
    public class HomeController : Controller
    {
        /// <summary>
        /// Handles the default Index action
        /// </summary>
        /// <returns>The Index View</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles the About action
        /// </summary>
        /// <returns>The About View</returns>
        public ActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Used to create a blank page to be used in creating Help wiki pages.
        /// </summary>
        /// <returns>A blank page</returns>
        public ActionResult Blank()
        {
            return View();
        }
    }
}
