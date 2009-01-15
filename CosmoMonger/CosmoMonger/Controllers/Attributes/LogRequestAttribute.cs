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

    public class LogRequestAttribute : ActionFilterAttribute, IActionFilter
    {
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
            Logger.Write(message, "Page Log", 1, 101, TraceEventType.Information, "Action Executed", props);
        }

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
            Logger.Write(message, "Page Log", 1, 101, TraceEventType.Information, "Action Executing", props);
        }
    }
}