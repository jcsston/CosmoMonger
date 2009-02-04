//-----------------------------------------------------------------------
// <copyright file="TradeController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// This controller deals will all trade related actions such as listing 
    /// available goods, buying goods, and selling goods.
    /// </summary>
    public class TradeController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public TradeController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public TradeController(GameManager manager) 
            : base(manager)
        {
        }

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
            ViewData["CurrentSystem"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem;
            ViewData["SystemGoods"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.GetGoods();
            ViewData["ShipGoods"] = this.ControllerGame.CurrentPlayer.Ship.GetGoods();
            ViewData["CashCredits"] = this.ControllerGame.CurrentPlayer.CashCredits;
            ViewData["BankCredits"] = this.ControllerGame.CurrentPlayer.BankCredits;
            ViewData["FreeCargoSpace"] = this.ControllerGame.CurrentPlayer.Ship.CargoSpaceFree;

            return View();
        }

        /// <summary>
        /// This action is called when a player wants to Buy goods at a system.
        /// Buys goods via the SystemGood.Buy method, redirects to the ListGoods action.
        /// </summary>
        /// <param name="goodId">The good id.</param>
        /// <param name="quantity">The quantity of goods to buy.</param>
        /// <returns>Redirect to ListGoods action on success, ListGoods view on error</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BuyGoods(int goodId, int quantity)
        {
            SystemGood systemGood = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.GetGood(goodId);
            if (systemGood != null)
            {
                try
                {
                    systemGood.Buy(this.ControllerGame, quantity);
                    return RedirectToAction("ListGoods");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("quantity", ex);
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("quantity", ex);
                }
            }
            else
            {
                ModelState.AddModelError("goodId", "Good is not sold in the system");
            }

            return View();
        }

        /// <summary>
        /// This action is called when a player wants to Sell goods at a system.
        /// Sells goods via the ShipGood.Sell method, redirects to the ListGoods action.
        /// </summary>
        /// <param name="goodId">The good id.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns>Redirect to ListGoods action on success, ListGoods view on error</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellGoods(int goodId, int quantity)
        {
            ShipGood shipGood = this.ControllerGame.CurrentPlayer.Ship.GetGood(goodId);
            if (shipGood != null)
            {
                try
                {
                    shipGood.Sell(this.ControllerGame, quantity);
                    return RedirectToAction("ListGoods");
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("quantity", ex);
                }
                catch (InvalidOperationException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("goodId", ex);
                }
            }
            else
            {
                ModelState.AddModelError("goodId", "Good is not bought in the system");
            }

            return View();
        }

        /// <summary>
        /// Fetches a price table of goods showing the price of goods at all systems in the galaxy.
        /// </summary>
        /// <returns>The PriceTable View</returns>
        public ActionResult PriceTable()
        {
            ViewData["CurrentSystem"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem;
            ViewData["PriceTable"] = this.ControllerGame.GetPriceTable();

            return View();
        }

        /// <summary>
        /// This action gets a graph of system good prices
        /// </summary>
        /// <returns>The Index action result</returns>
        public ActionResult GraphGoodPrice(int systemId)
        {
            CosmoSystem system = this.ControllerGame.GetSystem(systemId);
            if (system != null)
            {
                SystemGood [] goods = system.GetGoods();

                //ViewData["goodId"] = new SelectList(this.ControllerGame.GetGoods(), "GoodId", "Name", goodId);
                ViewData["systemId"] = new SelectList(this.ControllerGame.GetSystems(), "SystemId", "Name", systemId);
                ViewData["Goods"] = goods;
                // Get the price history for each system good
                ViewData["PriceHistory"] = goods.Select(g => g.GetPriceHistory());
                
                return View();
            }
            else
            {
                ModelState.AddModelError("systemId", "Invalid System");
            }

            // Show the default action
            return RedirectToAction("Index");
        }
    }
}
