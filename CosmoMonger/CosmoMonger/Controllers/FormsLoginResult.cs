namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    public class FormsLoginResult : ActionResult
    {
        private string userName;
        private string userData;
        private bool persistentCookie;

        public FormsLoginResult(string userName)
            : this(userName, /* persistentCookie */ true)
        {
        }

        public FormsLoginResult(string userName, bool persistentCookie)
        {
            if (String.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            this.userName = userName;
            this.persistentCookie = persistentCookie;
        }

        public bool PersistentCookie
        {
            get { return this.persistentCookie; }
        }

        public string UserData
        {
            get { return this.userData; }
            set { this.userData = value; }
        }

        public string UserName
        {
            get { return this.userName; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;

            if (String.IsNullOrEmpty(this.userData))
            {
                FormsAuthentication.SetAuthCookie(this.userName, this.persistentCookie);
            }
            else
            {
                FormsAuthenticationTicket ticket =
                    new FormsAuthenticationTicket(1, this.userName,
                                                  DateTime.Now, DateTime.Now.AddMinutes(5),
                                                  this.persistentCookie,
                                                  this.userData,
                                                  FormsAuthentication.FormsCookiePath);
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                                   encryptedTicket);
                cookie.HttpOnly = true;
                cookie.Secure = FormsAuthentication.RequireSSL;
                cookie.Path = FormsAuthentication.FormsCookiePath;
                if (FormsAuthentication.CookieDomain != null)
                {
                    cookie.Domain = FormsAuthentication.CookieDomain;
                }

                Logger.Write("Storing user login cookie", "Controller", 10, 0, TraceEventType.Verbose, "Storing cookie in FormsLoginResult.ExecuteResult",
                    new Dictionary<string, object>
                {
                    { "Cookie", cookie },
                    { "UserName", this.userName },
                    { "FormsCookiePath", FormsAuthentication.FormsCookiePath },
                    { "CookieDomain", FormsAuthentication.CookieDomain },
                    { "EncryptedTicket", encryptedTicket }
                }
                );

                response.Cookies.Add(cookie);
            }

            string redirectUrl = FormsAuthentication.GetRedirectUrl(this.userName, this.persistentCookie);
            Logger.Write("Redirecting login user", "Controller", 10, 0, TraceEventType.Verbose, "URL Redirect in FormsLoginResult.ExecuteResult",
                new Dictionary<string, object>
                {
                    { "RedirectURL", redirectUrl },
                    { "UserName", this.userName },
                    { "PersistentCookie", this.persistentCookie }
                }
            );
            response.Redirect(redirectUrl);
        }
    }
}