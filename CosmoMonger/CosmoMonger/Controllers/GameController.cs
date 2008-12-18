namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using CosmoMonger.Models;

    [Authorize]
    public class GameController : Controller
    {
        private GameManager gameManager = null;

        protected GameManager ControllerGame
        {
            get
            {
                if (gameManager == null)
                {
                    gameManager = new GameManager(User.Identity.Name);
                }
                return gameManager;
            }
        }
        /*
        public ActionResult Index()
        {
            // Add action logic here
            throw new NotImplementedException();
        }
        */
    }
}
