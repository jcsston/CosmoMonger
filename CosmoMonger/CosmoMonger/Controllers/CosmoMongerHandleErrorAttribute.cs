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
            if (filterContext != null && !filterContext.ExceptionHandled && filterContext.HttpContext.IsCustomErrorEnabled)
            {
                Exception innerException = filterContext.Exception;

                bool rethrow = ExceptionPolicy.HandleException(innerException, "Page Policy");
                if (rethrow)
                {
                    throw innerException;
                }
            }
            base.OnException(filterContext);
        }
    }
}
