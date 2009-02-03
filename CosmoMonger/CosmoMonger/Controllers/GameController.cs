//-----------------------------------------------------------------------
// <copyright file="GameController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
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
    /// This is the base controller for all game related controllers.
    /// Any user has to be authorized to access this controller.
    /// </summary>
    [Authorize]
    [ExceptionPolicyAttribute]
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
                            object lastNpcUpdate = this.ControllerContext.HttpContext.Application["LastNpcUpdate"];
                            if (lastNpcUpdate == null || (DateTime.Now - (DateTime)lastNpcUpdate).TotalSeconds > 5)
                            {
                                // Spawn thread to do NPC actions
                                Thread npcThread = new Thread(new ThreadStart(CosmoManager.DoPendingNPCActions));
                                npcThread.Name = "NPC Thread";
                                npcThread.Start();

                                // Update NPC Counter
                                this.ControllerContext.HttpContext.Application["LastNpcUpdate"] = DateTime.Now;
                            }
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
            if (this.ControllerGame.CurrentPlayer == null && filterContext.Controller.GetType() != typeof(PlayerController))
            {
                // Redirect to the CreatePlayer action
                filterContext.HttpContext.Response.Redirect(this.Url.Action("CreatePlayer", "Player"));
            }
            else
            {
                // Update the player playtime
                if (this.ControllerGame.CurrentPlayer != null)
                {
                    this.ControllerGame.CurrentPlayer.UpdatePlayTime();
                }

                base.OnActionExecuting(filterContext);
            }
        }
    }
}
