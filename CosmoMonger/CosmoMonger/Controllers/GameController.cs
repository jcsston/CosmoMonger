//-----------------------------------------------------------------------
// <copyright file="GameController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.UI;
    using CosmoMonger.Controllers.Attributes;
    using CosmoMonger.Models;

    /// <summary>
    /// This is the base controller for all game related controllers.
    /// Any user has to be authorized to access this controller.
    /// </summary>
    [Authorize]
    [ExceptionPolicy]
    [LogRequest]
    [OutputCache(Location = OutputCacheLocation.None)]
    public class GameController : Controller
    {
        /// <summary>
        /// Holds the GameManger object for this controller
        /// </summary>
        private GameManager gameManager = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// This is the default constructor that doesn't really to anything.
        /// </summary>
        public GameController()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// This constructor is used for unit testing purposes.
        /// </summary>
        /// <param name="manager">The game manager object to use.</param>
        public GameController(GameManager manager)
        {
            this.gameManager = manager;
        }

        /// <summary>
        /// Gets the GameManager for the current player in the context of this controller.
        /// </summary>
        /// <value>The GameManager object for this controller.</value>
        protected GameManager ControllerGame
        {
            get
            {
                if (this.gameManager == null)
                {
                    if (User != null && this.User.Identity.IsAuthenticated)
                    {
                        this.gameManager = new GameManager(this.User.Identity.Name);

                        // Check if we need to do NPC AI processing
                        lock (this.ControllerContext.HttpContext.Application.SyncRoot) 
                        {
                            // Check if the npc thread has been started
                            Thread npcThread = (Thread)this.ControllerContext.HttpContext.Application["NpcThread"];
                            if (npcThread == null || npcThread.IsAlive) 
                            {
                                // Startup the NPC thread
                                npcThread = new Thread(new ThreadStart(CosmoManager.NpcThreadEntry));
                                npcThread.IsBackground = true;
                                npcThread.Start();

                                // Keep a reference so we can detect if it's been started or not
                                this.ControllerContext.HttpContext.Application["NpcThread"] = npcThread;

                                // Wait for the thread to startup
                                while (!npcThread.IsAlive)
                                {
                                    Thread.Sleep(0);
                                }
                            }
                            /*
                            object lastNpcUpdate = this.ControllerContext.HttpContext.Application["LastNpcUpdate"];
                            if (lastNpcUpdate == null || (DateTime.UtcNow - (DateTime)lastNpcUpdate).TotalSeconds > 5)
                            {
                                // Queue thread to do NPC actions
                                ThreadPool.QueueUserWorkItem(new WaitCallback(CosmoManager.DoPendingNPCActions));
                                
                                // Update NPC Counter
                                this.ControllerContext.HttpContext.Application["LastNpcUpdate"] = DateTime.UtcNow;
                            }
                            */
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("This property should never be accessed by an un-authenticated user");
                    }
                }

                return this.gameManager;
            }
        }

        /// <summary>
        /// Called before an action has executed.
        /// This override is used to redirect users without an active player to the PlayerController.CreatePlayer action.
        /// </summary>
        /// <param name="filterContext">The context of the executing action.</param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if the user has a current player and is not trying to create a player
            Type controllerType = filterContext.Controller.GetType();
            if (this.ControllerGame.CurrentPlayer == null)
            {
                if (controllerType != typeof(PlayerController))
                {
                    // Redirect to the CreatePlayer action
                    filterContext.HttpContext.Response.Redirect(this.Url.Action("CreatePlayer", "Player"));
                }
            }
            else
            {
                if (this.ControllerGame.CurrentPlayer.Ship.InProgressCombat != null && controllerType != typeof(CommunicationController) && controllerType != typeof(CombatController) && controllerType != typeof(AdminController))
                {
                    // The player is currently in combat, redirect to combat start page
                    filterContext.HttpContext.Response.Redirect(this.Url.Action("CombatStart", "Combat"));
                }
                else
                {
                    // Check that the session for the user matches
                    if (this.Session.SessionID != this.ControllerGame.CurrentUser.SessionID)
                    {
                        // Redirect to the Logout page
                        filterContext.HttpContext.Response.Redirect(this.Url.Action("Logout", "Account"));
                    }

                    // Update the player playtime
                    Player currentPlayer = this.ControllerGame.CurrentPlayer;
                    if (currentPlayer != null)
                    {
                        currentPlayer.UpdatePlayTime();

                        // Redirect to the dead screen if the player has died
                        if (!currentPlayer.Alive)
                        {
                            filterContext.HttpContext.Response.Redirect(this.Url.Action("Dead", "Player"));
                        }
                    }

                    base.OnActionExecuting(filterContext);
                }
            }
        }
    }
}
