//-----------------------------------------------------------------------
// <copyright file="HomeController.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
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
            ViewData["Title"] = "CosmoMonger";
            
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

        /// <summary>
        /// This action processes pending background events that need processing.
        /// Such as NPC actions or good production/consumption.
        /// </summary>
        /// <returns>The Index action result</returns>
        public ActionResult ProcessEvents()
        {
            CosmoManager manager = new CosmoManager();
            
            // Do NPC work
            manager.DoPendingNPCActions();

            // Show the normal home page
            return RedirectToAction("Index");
        }
    }
}
