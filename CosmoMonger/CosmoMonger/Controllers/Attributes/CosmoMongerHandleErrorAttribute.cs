//-----------------------------------------------------------------------
// <copyright file="CosmoMongerHandleErrorAttribute.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers.Attributes
{
    using System;
    using System.Web.Mvc;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

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
