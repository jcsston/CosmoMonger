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

    /// <summary>
    /// This attribute adds exception handling to the controller actions.
    /// If an exception is detected, it is handled by the ExceptionPolicy and a 
    /// formatted message is displayed to the user.
    /// </summary>
    public class CosmoMongerHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception is thrown in the view or action.
        /// </summary>
        /// <param name="filterContext">The filter context of the Exception.</param>
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
