//-----------------------------------------------------------------------
// <copyright file="GameController.cs" company="CosmoMonger">
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
    }
}
