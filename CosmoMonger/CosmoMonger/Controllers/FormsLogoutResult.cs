//-----------------------------------------------------------------------
// <copyright file="FormsLogoutResult.cs" company="CosmoMonger">
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
    using System.Web.Security;

    public class FormsLogoutResult : ActionResult
    {
        private string url;

        public FormsLogoutResult()
            : this(FormsAuthentication.DefaultUrl)
        {
        }

        public FormsLogoutResult(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url", "Redirect URL cannot be empty");
            }
            this.url = url;
        }

        public string Url
        {
            get { return this.url; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            FormsAuthentication.SignOut();
            context.HttpContext.Response.Redirect(this.url);
        }
    }

}
