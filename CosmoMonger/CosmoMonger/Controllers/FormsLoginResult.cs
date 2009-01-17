namespace CosmoMonger.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class FormsLoginResult : ActionResult
    {
        private string userName;
        private string userData;
        private bool persistentCookie;

        public FormsLoginResult(string userName)
            : this(userName, /* persistentCookie */ false)
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
                                                  DateTime.Now, DateTime.Now.AddMinutes(15),
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

                response.Cookies.Add(cookie);
            }

            response.Redirect(FormsAuthentication.GetRedirectUrl(this.userName, this.persistentCookie));
        }
    }
}