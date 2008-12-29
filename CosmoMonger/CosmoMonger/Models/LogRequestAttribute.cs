namespace CosmoMonger.Models
{
    using System.Web.Mvc;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using System.Collections.Generic;
    using System.Threading;
    using System.Diagnostics;

    public class LogRequestAttribute : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            /*
            ICollection<string> props = (ICollection<string>)new
            {
                Action = filterContext.Result,
                Controller = filterContext.Controller.ToString(),
                TimeStamp = filterContext.HttpContext.Timestamp,
                IPAddress = filterContext.HttpContext.Request.UserHostAddress
            };*/
            string message = filterContext.RouteData.Values["controller"] + "." + filterContext.RouteData.Values["action"];
            Logger.Write(message, "Page Log", 1, 101, TraceEventType.Information, "Action Executed");
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            string message = filterContext.RouteData.Values["controller"] + "." + filterContext.RouteData.Values["action"];
            Logger.Write(message, "Page Log", 1, 101, TraceEventType.Information, "Action Executing");
        }
    }
}