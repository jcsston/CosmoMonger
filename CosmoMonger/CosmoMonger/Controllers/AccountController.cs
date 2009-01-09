namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Security.Principal;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using System.Web.UI;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Recaptcha;
    using System.Web.Routing;

    /// <summary>
    /// This controller manages user account creation, login, and logout
    /// </summary>
    [CosmoMongerHandleError]
    [OutputCache(Location = OutputCacheLocation.None)]
    public class AccountController : Controller
    {
        /// <summary>
        /// This constructor is used by the MVC framework to instantiate the controller using
        /// the default forms authentication and membership providers.
        /// </summary>
        public AccountController()
            : this(null, null)
        {
        }

        /// <summary>
        /// This constructor is not used by the MVC framework but is instead provided for ease
        /// of unit testing this type. See the comments at the end of this file for more
        /// information.
        /// </summary>
        public AccountController(IFormsAuthentication formsAuth, MembershipProvider provider)
        {
            FormsAuth = formsAuth ?? new FormsAuthenticationWrapper();
            Provider = provider ?? Membership.Provider;
        }

        public IFormsAuthentication FormsAuth
        {
            get;
            private set;
        }

        public MembershipProvider Provider
        {
            get;
            private set;
        }

        [Authorize]
        public ActionResult ChangePassword()
        {

            ViewData["Title"] = "Change Password";
            ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            ViewData["Title"] = "Change Password";
            ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

            // Basic parameter validation
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.");
            }
            if (newPassword == null || newPassword.Length < Provider.MinRequiredPasswordLength)
            {
                ModelState.AddModelError("newPassword",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a new password of {0} or more characters.",
                         Provider.MinRequiredPasswordLength));
            }
            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            if (ModelState.IsValid)
            {
                // Attempt to change password
                bool changeSuccessful = false;
                try
                {
                    changeSuccessful = Provider.ChangePassword(User.Identity.Name, currentPassword, newPassword);
                }
                catch
                {
                    // An exception is thrown if the new password does not meet the provider's requirements
                }

                if (changeSuccessful)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult ChangePasswordSuccess()
        {

            ViewData["Title"] = "Change Password";

            return View();
        }

        public ActionResult Login()
        {

            ViewData["Title"] = "Login";

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string username, string password, bool rememberMe, string returnUrl)
        {

            ViewData["Title"] = "Login";

            // Basic parameter validation
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }

            if (ViewData.ModelState.IsValid)
            {
                // Attempt to login
                MembershipUser user = Provider.GetUser(username, false);
                if (user != null)
                {

                    bool loginSuccessful = Provider.ValidateUser(username, password);

                    if (loginSuccessful)
                    {
                        FormsAuth.SetAuthCookie(username, rememberMe);
                        if (!String.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else if (!user.IsApproved)
                    {
                        ModelState.AddModelError("_FORM", "The username provided has not been verified. Check your e-mail for the verification e-mail.");
                    }
                    else if (user.IsLockedOut)
                    {
                        ModelState.AddModelError("_FORM", "The username provided has been locked. Contact the administrator.");
                    }
                    else
                    {
                        ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
                    }
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The username provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["rememberMe"] = rememberMe;
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuth.SignOut();

            return RedirectToAction("Index", "Home");
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        public ActionResult Register()
        {

            ViewData["Title"] = "Register";
            ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string username, string email, string password, string confirmPassword)
        {

            ViewData["Title"] = "Register";
            ViewData["PasswordLength"] = Provider.MinRequiredPasswordLength;

            // Basic parameter validation
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.");
            }
            if (password == null || password.Length < Provider.MinRequiredPasswordLength)
            {
                ModelState.AddModelError("password",
                    String.Format(CultureInfo.CurrentCulture,
                         "You must specify a password of {0} or more characters.",
                         Provider.MinRequiredPasswordLength));
            }
            if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
            }

            // We don't check the captcha if running localhost and no challenge was given
            if (this.Request.UserHostAddress == "127.0.0.1" && this.Request.Form["recaptcha_challenge_field"] != null)
            {
                // Check the captcha response
                RecaptchaValidator humanValidator = new RecaptchaValidator();
                humanValidator.PrivateKey = ConfigurationManager.AppSettings["RecaptchaPrivateKey"];
                humanValidator.RemoteIP = this.Request.UserHostAddress;
                humanValidator.Challenge = this.Request.Form["recaptcha_challenge_field"];
                humanValidator.Response = this.Request.Form["recaptcha_response_field"];

                RecaptchaResponse humanResponse = humanValidator.Validate();
                if (!humanResponse.IsValid)
                {
                    Dictionary<string, object> props = new Dictionary<string, object>
                { 
                    { "PrivateKey", humanValidator.PrivateKey },
                    { "RemoteIP", humanValidator.RemoteIP },
                    { "Challenge", humanValidator.Challenge },
                    { "Response", humanValidator.Response },
                    { "IsValid", humanResponse.IsValid },
                    { "ErrorCode", humanResponse.ErrorCode }
                };
                    Logger.Write("Failed reCAPTCHA attempt", "Controller", 100, 1042, TraceEventType.Verbose, "Failed reCAPTCHA attempt", props);
                    ModelState.AddModelError("recaptcha", "reCAPTCHA failed to verify");
                }
            }

            if (ViewData.ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                CosmoMongerMembershipUser newUser = (CosmoMongerMembershipUser)Provider.CreateUser(username, password, email, null, null, false, null, out createStatus);

                if (newUser != null)
                {
                    return RedirectToAction("SendVerificationCode", new RouteValueDictionary(new {
                        username = username
                    }));
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        public ActionResult SendVerificationCode(string username)
        {
            CosmoMongerMembershipUser verifyUser = (CosmoMongerMembershipUser)Provider.GetUser(username, false);
            if (verifyUser != null)
            {
                string baseVerificationUrl = this.Request.Url.GetLeftPart(UriPartial.Authority)  + this.Url.Action("VerifyEmail") + "?username=" + this.Url.Encode(username) + "&verificationCode=";
                if (verifyUser.SendVerificationCode(baseVerificationUrl))
                {
                    ViewData["Title"] = "Sent Verification Code";
                    return View("SentVerificationCode");
                }
                else
                {
                    // Failed to send e-mail
                    ModelState.AddModelError("_FORM", "Failed to send verification e-mail. Please try again, if the problem persists, please contact admin@cosmomonger.com.");
                }
            }
            else
            {
                // Username is invalid
                ModelState.AddModelError("username", "Invalid username");
            }

            // If we got this far, something failed
            ViewData["Title"] = "Send Verification Code";
            return View();
        }

        public ActionResult VerifyEmail(string username, string verificationCode)
        {
            CosmoMongerMembershipUser checkUser = (CosmoMongerMembershipUser)Provider.GetUser(username, false);
            if (checkUser != null)
            {
                if (checkUser.VerifyEmail(verificationCode))
                {
                    ViewData["Title"] = "Verified Email";
                    ViewData["UserName"] = checkUser.UserName;
                    ViewData["Email"] = checkUser.Email;
                    return View("VerifiedEmail");
                }
                else
                {
                    ModelState.AddModelError("verificationCode", "Invalid verification code");
                }
            }
            else
            {
                ModelState.AddModelError("username", "Invalid username");
            }

            ViewData["Title"] = "Verify Email";
            return View();
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }

    // The FormsAuthentication type is sealed and contains static members, so it is difficult to
    // unit test code that calls its members. The interface and helper class below demonstrate
    // how to create an abstract wrapper around such a type in order to make the AccountController
    // code unit testable.

    public interface IFormsAuthentication
    {
        void SetAuthCookie(string userName, bool createPersistentCookie);
        void SignOut();
    }

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
