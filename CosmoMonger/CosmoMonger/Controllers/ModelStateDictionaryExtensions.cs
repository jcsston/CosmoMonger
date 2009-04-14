//-----------------------------------------------------------------------
// <copyright file="ModelStateDictionaryExtensions.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>
    /// Extensions to the ModelStateDictionary class
    /// </summary>
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Adds a model error to the state dictionary with the value causing the error
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <param name="key">The ViewData/ModelData key.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="attemptedValue">The attempted value of the ViewData/ModelData.</param>
        public static void AddModelError(this ModelStateDictionary modelState, string key, string errorMessage, object attemptedValue)
        {
            modelState.AddModelError(key, errorMessage);
            modelState.SetModelValue(key, new ValueProviderResult(attemptedValue, (attemptedValue ?? String.Empty).ToString(), null));
        }
    }
}
