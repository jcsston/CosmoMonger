namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.UI;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This controller handles all combat related tasks such as selecting a ship to attack,
    /// taking combat turns, and victory/defeat conditions.
    /// </summary>
    public class CombatController : GameController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CombatController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public CombatController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CombatController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public CombatController(GameManager manager) 
            : base(manager)
        {
        }

        /// <summary>
        /// Index action, redirects to Attack action.
        /// </summary>
        /// <returns>Redirect to the Atack action</returns>
        public ActionResult Index()
        {
            return RedirectToAction("Attack");
        }

        /// <summary>
        /// List ships to attack
        /// </summary>
        /// <returns>Attack view filled in with list of ships to attack. Redirect to CombatStart if a combat is in progress.</returns>
        public ActionResult Attack()
        {
            Ship playerShip = this.ControllerGame.CurrentPlayer.Ship;
            // Check if there is a combat in progress
            if (playerShip.InProgressCombat != null)
            {
                // Redirect to combat start
                return RedirectToAction("CombatStart");
            }

            ViewData["Ships"] = playerShip.CosmoSystem.GetShipsInSystem().Where(s => s != playerShip);
            ViewData["ShipsToAttack"] = playerShip.GetShipsToAttack();

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attack(int shipId)
        {
            Ship shipToAttack = this.ControllerGame.GetShip(shipId);
            if (shipToAttack != null)
            {
                try
                {
                    this.ControllerGame.CurrentPlayer.Ship.Attack(shipToAttack);
                    return RedirectToAction("CombatStart");
                }
                catch (InvalidOperationException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    ModelState.AddModelError("shipId", ex.Message, shipId);
                }
            }
            else
            {
                ModelState.AddModelError("shipId", "Invalid ship", shipId);
            }
            
            return View();
        }

        public ActionResult CombatStart()
        {
            Combat currentCombat = this.ControllerGame.CurrentPlayer.Ship.InProgressCombat;
            if (currentCombat != null)
            {
                ViewData["Combat"] = currentCombat;
                ViewData["PlayerName"] = this.ControllerGame.CurrentPlayer.Name;
                ViewData["PlayerShip"] = this.ControllerGame.CurrentPlayer.Ship;
                if (this.ControllerGame.CurrentPlayer.Ship == currentCombat.AttackerShip)
                {
                    ViewData["EnemyName"] = currentCombat.DefenderShip.Players.Single().Name;
                    ViewData["EnemyShip"] = currentCombat.DefenderShip;
                }
                else
                {
                    ViewData["EnemyName"] = currentCombat.AttackerShip.Players.Single().Name;
                    ViewData["EnemyShip"] = currentCombat.AttackerShip;
                }

                return View();
            }
            else
            {
                return View("CombatNone");
            }
        }

        public ActionResult CombatComplete(int combatId)
        {
            Combat combat = this.ControllerGame.GetCombat(combatId);
            if (combat != null)
            {
                ViewData["CreditsLooted"] = combat.CreditsLooted;

                Ship playerShip = this.ControllerGame.CurrentPlayer.Ship;
                switch (combat.Status)
                {
                    case Combat.CombatStatus.ShipDestroyed:
                        if (combat.ShipTurn == playerShip)
                        {
                            ViewData["Message"] = "You have won";
                            ViewData["CargoLooted"] = combat.CombatGoods;
                        }
                        else
                        {
                            ViewData["Message"] = "You have lost";
                            ViewData["CargoLost"] = combat.CombatGoods;
                        }
                        break;

                    case Combat.CombatStatus.CargoPickup:
                        if (combat.ShipTurn == playerShip)
                        {
                            ViewData["Message"] = "Picked up jettisioned cargo";
                            ViewData["CargoLooted"] = combat.CombatGoods;
                        }
                        else
                        {
                            ViewData["Message"] = "You escaped by jettisoning cargo";
                            ViewData["CargoLost"] = combat.CombatGoods;
                        }
                        break;

                    case Combat.CombatStatus.ShipFled:
                        if (combat.ShipTurn == playerShip)
                        {
                            ViewData["Message"] = "You have escaped";
                            ViewData["FinalImage"] = "RunningChicken.png";
                        }
                        else
                        {
                            ViewData["Message"] = "Enemy has escaped";
                        }
                        break;

                    case Combat.CombatStatus.ShipSurrendered:
                        if (combat.ShipTurn == playerShip)
                        {
                            ViewData["Message"] = "You have captured the opposing player";
                            ViewData["CargoLooted"] = combat.CombatGoods;
                        }
                        else
                        {
                            ViewData["Message"] = "You have surrendered";
                            ViewData["CargoLost"] = combat.CombatGoods;
                        }
                        break;
                }

                return View();
            }
            else
            {
                return View("NoCombat");
            }
        }

        /// <summary>
        /// Check if the ship has entered combat
        /// </summary>
        /// <returns>A JSON data object with combat fields.</returns>
        public JsonResult CombatPending()
        {
            Ship currentShip = this.ControllerGame.CurrentPlayer.Ship;
            bool inCombat = (currentShip.InProgressCombat != null);

            return Json(new { combat = inCombat });
        }

        public JsonResult CombatStatus(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                // Check that the player turn is not over
                selectedCombat.CheckTurnTimeLeft();

                return Json(BuildCombatStatus(selectedCombat));
            }

            return Json(false);
        }

        public JsonResult FireWeapon(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                string message = null;

                // Check that it is the current players turn
                if (selectedCombat.ShipTurn == this.ControllerGame.CurrentPlayer.Ship)
                {
                    try
                    {
                        bool weaponHit = selectedCombat.FireWeapon();
                        if (!weaponHit)
                        {
                            message = "Your shot missed";
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Combat is over
                        message = ex.Message;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Not enough turn points
                        message = ex.Message;
                    }
                }

                return Json(new { message = message, status = BuildCombatStatus(selectedCombat) });
            }

            return Json(false);
        }

        public JsonResult ChargeJumpDrive(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                string message = null;

                // Check that it is the current players turn
                if (selectedCombat.ShipTurn == this.ControllerGame.CurrentPlayer.Ship)
                {
                    try
                    {
                        selectedCombat.ChargeJumpDrive();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Combat is over
                        message = ex.Message;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Not enough turn points
                        message = ex.Message;
                    }
                }

                return Json(new { message = message, status = BuildCombatStatus(selectedCombat) });
            }

            return Json(false);
        }

        public JsonResult JettisonCargo(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                string message = null;

                // Check that it is the current players turn
                if (selectedCombat.ShipTurn == this.ControllerGame.CurrentPlayer.Ship)
                {
                    try
                    {
                        selectedCombat.JettisonCargo();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Combat is over
                        message = ex.Message;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Not enough turn points
                        message = ex.Message;
                    }
                }

                return Json(new { message = message, status = BuildCombatStatus(selectedCombat) });
            }

            return Json(false);
        }

        public JsonResult PickupCargo(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                string message = null;

                // Check that it is the current players turn
                if (selectedCombat.ShipTurn == this.ControllerGame.CurrentPlayer.Ship)
                {
                    try
                    {
                        selectedCombat.PickupCargo();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Combat is over
                        message = ex.Message;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Not enough turn points
                        message = ex.Message;
                    }
                }

                return Json(new { message = message, status = BuildCombatStatus(selectedCombat) });
            }

            return Json(false);
        }


        public JsonResult OfferSurrender(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                string message = null;

                // Check that it is the current players turn
                if (selectedCombat.ShipTurn == this.ControllerGame.CurrentPlayer.Ship)
                {
                    try
                    {
                        selectedCombat.OfferSurrender();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Surrender already offered
                        message = ex.Message;
                    }
                }

                return Json(new { message = message, status = BuildCombatStatus(selectedCombat) });
            }

            return Json(false);
        }

        public JsonResult AcceptSurrender(int combatId)
        {
            Combat selectedCombat = this.ControllerGame.GetCombat(combatId);
            if (selectedCombat != null)
            {
                string message = null;

                // Check that it is the current players turn
                if (selectedCombat.ShipTurn == this.ControllerGame.CurrentPlayer.Ship)
                {
                    try
                    {
                        selectedCombat.AcceptSurrender();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // No surrender offered?
                        message = ex.Message;
                    }
                }

                return Json(new { message = message, status = BuildCombatStatus(selectedCombat) });
            }

            return Json(false);
        }

        /// <summary>
        /// Builds a combat status object for JSON
        /// </summary>
        /// <param name="combat">The combat object to build from.</param>
        /// <returns>
        /// A combat status data object, suitable for JSON
        /// Fields:
        /// int playerHull - amount of damage to hull
        /// int playerShield
        /// int enemyHull - amount of damage to hull
        /// int enemyShield
        /// bool turn - True is player's turn, false is other players turn
        /// bool surrendered - When True the other player has offered a surrender
        /// int cargoJettisoned - When greater than 0 the other player has jettisoned their cargo, number is the number of cargo items jettisoned
        /// int jumpDriveCharge - charge of JumpDrive
        /// int turnPoints - Number of turn points left
        /// double timeLeft - Number of seconds left in current turn action
        /// bool complete - true when combat is complete
        /// </returns>
        private object BuildCombatStatus(Combat combat)
        {
            Ship playerShip, enemyShip;
            if (this.ControllerGame.CurrentPlayer.Ship == combat.AttackerShip)
            {
                playerShip = combat.AttackerShip;
                enemyShip = combat.DefenderShip;
            }
            else
            {
                playerShip = combat.DefenderShip;
                enemyShip = combat.AttackerShip;
            }

            return new { 
                playerHull = playerShip.DamageHull, 
                playerShield = playerShip.DamageShield,
                enemyHull = enemyShip.DamageHull,
                enemyShield = enemyShip.DamageShield,
                turn = (playerShip == combat.ShipTurn),
                surrendered = combat.Surrendered,
                cargoJettisoned = combat.CargoJettisonedCount,
                jumpDriveCharge = playerShip.CurrentJumpDriveCharge,
                turnPoints = combat.TurnPointsLeft,
                timeLeft = combat.TurnTimeLeft.TotalSeconds,
                complete = (combat.Status != Combat.CombatStatus.Incomplete)
            };
        }
    }
}
