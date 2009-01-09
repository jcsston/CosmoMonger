namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Logging;

    public class CosmoMongerHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            if (filterContext.HttpContext.Response.StatusCode == 500 && filterContext.ExceptionHandled)
            {
                ViewResult result = (ViewResult)filterContext.Result;
                try
                {
                    ExceptionPolicy.HandleException(filterContext.Exception, "Page Policy");
                }
                catch (ApplicationException ex)
                {
                    result.ViewData["Title"] = "Space-time anomaly detected";
                    result.ViewData["Message"] = ex.Message;
                }
            }
        }
    }
}
