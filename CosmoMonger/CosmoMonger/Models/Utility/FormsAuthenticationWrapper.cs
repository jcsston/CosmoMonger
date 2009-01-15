namespace CosmoMonger.Models.Utility
{
    using System;
    using System.Web.Security;

    /// <summary>
    /// 
    /// </summary>
    public class FormsAuthenticationWrapper : IFormsAuthentication
    {
        public void SetAuthCookie(string userName, bool createPersistentCookie)
        {
            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}