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
        private string _url;

        public FormsLogoutResult()
            : this(FormsAuthentication.DefaultUrl)
        {
        }

        public FormsLogoutResult(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            FormsAuthentication.SignOut();
            context.HttpContext.Response.Redirect(_url);
        }
    }

}
