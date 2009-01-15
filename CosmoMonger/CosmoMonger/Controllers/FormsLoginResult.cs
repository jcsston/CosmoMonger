namespace CosmoMonger.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;

    public class FormsLoginResult : ActionResult
    {
        private string _userName;
        private string _userData;
        private bool _persistentCookie;

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
            _userName = userName;
            _persistentCookie = persistentCookie;
        }

        public bool PersistentCookie
        {
            get { return _persistentCookie; }
        }

        public string UserData
        {
            get { return _userData; }
            set { _userData = value; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public override void ExecuteResult(ControllerContext context)
        {
            HttpResponseBase response = context.HttpContext.Response;

            if (String.IsNullOrEmpty(_userData))
            {
                FormsAuthentication.SetAuthCookie(_userName, _persistentCookie);
            }
            else
            {
                FormsAuthenticationTicket ticket =
                    new FormsAuthenticationTicket(1, _userName,
                                                  DateTime.Now, DateTime.Now.AddMinutes(30),
                                                  _persistentCookie,
                                                  _userData,
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

            response.Redirect(FormsAuthentication.GetRedirectUrl(_userName, _persistentCookie));
        }
    }
}