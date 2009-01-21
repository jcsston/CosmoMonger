namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;

    /// <summary>
    /// 
    /// </summary>
    public class BankController : GameController
    {
        /// <summary>
        /// The Index action for the Bank controller.
        /// Redirects to the Bank action.
        /// </summary>
        /// <returns>Redirect to the Bank action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("Bank");
        }

        /// <summary>
        /// The primary action for the Bank controller.
        /// Returns a View filled in with the current players bank stats.
        /// </summary>
        /// <returns>The Bank View</returns>
        public ActionResult Bank()
        {
            ViewData["Title"] = "Bank";
            ViewData["CurrentPlayer"] = this.ControllerGame.CurrentPlayer;
            ViewData["BankAvailable"] = this.ControllerGame.CurrentPlayer.Ship.CosmoSystem.HasBank;

            return View();
        }

        /// <summary>
        /// Deposits the specified number of credits into the bank.
        /// </summary>
        /// <param name="depositCredits">The number of credits to deposit.</param>
        /// <returns>A redirect to the Index action</returns>
        public ActionResult Deposit(int depositCredits)
        {
            try
            {
                this.ControllerGame.CurrentPlayer.BankDeposit(depositCredits);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("_FORM", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ModelState.AddModelError("depositCredits", ex);
            }

            return RedirectToAction("Bank");
        }

        /// <summary>
        /// Withdraws the specified number of credits from the bank.
        /// </summary>
        /// <param name="withdrawCredits">The number of credits to withdraw.</param>
        /// <returns>A redirect to the Index action</returns>
        public ActionResult Withdraw(int withdrawCredits)
        {
            try
            {
                this.ControllerGame.CurrentPlayer.BankWithdraw(withdrawCredits);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("_FORM", ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ModelState.AddModelError("withdrawCredits", ex);
            }

            return RedirectToAction("Bank");
        }
    }
}
