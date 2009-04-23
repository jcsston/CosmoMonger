//-----------------------------------------------------------------------
// <copyright file="LogRequestAttribute.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers.Attributes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web.Mvc;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Logs the execution start and stop of actions on controllers
    /// </summary>
    public class LogRequestAttribute : ActionFilterAttribute, IActionFilter
    {
        /// <summary>
        /// Called after the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            Dictionary<string, object> props = new Dictionary<string, object>
            { 
                { "Controller", filterContext.Controller.ToString() },
                { "TimeStamp", filterContext.HttpContext.Timestamp },
                { "IPAddress", filterContext.HttpContext.Request.UserHostAddress },
                { "SessionID", filterContext.HttpContext.Session.SessionID }
            };
            string message = filterContext.RouteData.Values["controller"] + "." + filterContext.RouteData.Values["action"];
            Logger.Write(message, "Action Log", 100, 0, TraceEventType.Stop, "Action Executed", props);
        }

        /// <summary>
        /// Called before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, object> props = new Dictionary<string, object>
            { 
                { "Controller", filterContext.Controller.ToString() },
                { "TimeStamp", filterContext.HttpContext.Timestamp },
                { "IPAddress", filterContext.HttpContext.Request.UserHostAddress },
                { "SessionID", filterContext.HttpContext.Session.SessionID }
            };
            string message = filterContext.RouteData.Values["controller"] + "." + filterContext.RouteData.Values["action"];
            Logger.Write(message, "Action Log", 100, 0, TraceEventType.Start, "Action Executing", props);
        }
    }
}