//-----------------------------------------------------------------------
// <copyright file="GameController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
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
    /// This is the base controller for all game related controllers.
    /// Any user has to be authorized to access this controller.
    /// </summary>
    [Authorize]
    [CosmoMongerHandleErrorAttribute]
    public class GameController : Controller
    {
        /// <summary>
        /// Holds the GameManger object for this controller
        /// </summary>
        private GameManager gameManager = null;

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
                    if (User != null && User.Identity.IsAuthenticated)
                    {
                        this.gameManager = new GameManager(User.Identity.Name);
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
        /// Called before an action result has executed.
        /// This override is used to redirect users without an active player to the PlayerController.CreatePlayer action.
        /// </summary>
        /// <param name="filterContext">The context of the executing action result.</param>
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // Check if the user has a current player and is not trying to create a player
            if (this.ControllerGame.CurrentPlayer == null && filterContext.Controller.GetType() != typeof(PlayerController) && filterContext.RouteData.Values["action"].ToString() != "CreatePlayer")
            {
                // Redirect to the CreatePlayer action
                filterContext.HttpContext.Response.Redirect(this.Url.Action("CreatePlayer", "Player"));
            }
            else
            {
                base.OnResultExecuting(filterContext);
            }
        }
    }
}
