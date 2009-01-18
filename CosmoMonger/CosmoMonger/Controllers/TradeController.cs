//-----------------------------------------------------------------------
// <copyright file="TradeController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This controller deals will all trade related actions such as listing 
    /// available goods, buying goods, and selling goods.
    /// </summary>
    public class TradeController : GameController
    {
        /// <summary>
        /// Redirects to ListGoods action.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return RedirectToAction("ListGoods");
        }

        /// <summary>
        /// The system to list the goods from will be the current players system.
        /// Fetches the list of goods via the System.GetGoods method and passes the data to the ListGoods view.
        /// </summary>
        /// <returns></returns>
        public ActionResult ListGoods()
        {
            ViewData["Title"] = "List Goods";
            ViewData["SystemGoods"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.GetGoods();
            ViewData["ShipGoods"] = this.ControllerGame.CurrentPlayer.Ship.GetGoods();
            return View();
        }

        /// <summary>
        /// This action is called when a player wants to Buy goods at a system.
        /// Buys goods via the SystemGood.Buy method, redirects to the ListGoods action.
        /// </summary>
        /// <param name="goodId">The good id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public ActionResult BuyGoods(int goodId, int quantity)
        {
            SystemGood systemGood = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.GetGood(goodId);
            systemGood.Buy(this.ControllerGame, quantity);
            return RedirectToAction("ListGoods");
        }

        /// <summary>
        /// This action is called when a player wants to Sell goods at a system.
        /// Sells goods via the ShipGood.Sell method, redirects to the ListGoods action.
        /// </summary>
        /// <param name="goodId">The good id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public ActionResult SellGoods(int goodId, int quantity)
        {
            ShipGood shipGood = this.ControllerGame.CurrentPlayer.Ship.GetGood(goodId);
            shipGood.Sell(this.ControllerGame, quantity);
            return RedirectToAction("ListGoods");
        }
    }
}
