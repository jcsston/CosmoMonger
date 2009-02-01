namespace CosmoMonger
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            Logger.Write("CosmoMonger Application Start (r" + CosmoManager.GetCodeVersion().ToString() + " db" + CosmoManager.GetDatabaseVersion().ToString() + ")", "ASP.NET Core", 1000, 0, TraceEventType.Information, "CosmoMonger Application Start");
            RegisterRoutes(RouteTable.Routes);
        }
    }
}