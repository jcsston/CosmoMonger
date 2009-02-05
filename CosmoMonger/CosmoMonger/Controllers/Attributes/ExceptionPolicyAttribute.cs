//-----------------------------------------------------------------------
// <copyright file="ExceptionPolicyAttribute.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers.Attributes
{
    using System;
    using System.Threading;
    using System.Web.Mvc;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// This attribute adds exception handling to the controller actions.
    /// If an exception is detected, it is handled by the ExceptionPolicy and a 
    /// formatted message is displayed to the user.
    /// </summary>
    public class ExceptionPolicyAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception is thrown in the view or action.
        /// </summary>
        /// <param name="filterContext">The filter context of the Exception.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            try
            {
                ExceptionPolicy.HandleException(filterContext.Exception, "Page Policy");
            }
            catch (ApplicationException ex)
            {
                // Setup the view
                base.OnException(filterContext);

                // Fill in the view with the exception details
                ViewResult result = filterContext.Result as ViewResult;
                if (result != null)
                {
                    result.ViewData["Message"] = ex.Message;
                }
            }
        }
    }
}
