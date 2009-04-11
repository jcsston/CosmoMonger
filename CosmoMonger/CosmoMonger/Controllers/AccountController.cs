//-----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
// <author>Roger Boykin</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using CosmoMonger.Controllers.Attributes;
    using CosmoMonger.Models;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Recaptcha;

    /// <summary>
    /// This controller manages user account creation, login, and logout
    /// </summary>
    [ExceptionPolicy]
    [OutputCache(Location = OutputCacheLocation.None)]
    public class AccountController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the AccountController class.
        /// This constructor is used by the MVC framework to instantiate the controller using
        /// the default forms authentication and membership providers.
        /// </summary>
        public AccountController()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AccountController class.
        /// This constructor is not used by the MVC framework but is instead provided for ease
        /// of unit testing this type. See the comments on the IFormsAuthentication interface for more
        /// information.
        /// </summary>
        /// <param name="provider">The provider object to user.</param>
        public AccountController(MembershipProvider provider)
        {
            this.Provider = provider ?? Membership.Provider;
        }

        /// <summary>
        /// Gets the MembershipProvider used for auth.
        /// </summary>
        /// <value>Gets the MembershipProvider used for login/logout.</value>
        public MembershipProvider Provider
        {
            get;
            private set;
        }

        /// <summary>
        /// Logins this instance.
        /// </summary>
        /// <returns>The Login view</returns>
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Logins the specified username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>A redirection if successful, the Login view on failure.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            // Basic parameter validation
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "You must specify a username.", username);
            }

            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.", password);
            }

            if (ViewData.ModelState.IsValid)
            {
                // Attempt to login
                CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.Provider.GetUser(username, true);
                if (user != null)
                {
                    bool loginSuccessful = this.Provider.ValidateUser(username, password);

                    if (loginSuccessful)
                    {
                        if (this.Session != null)
                        {
                            // Store something in the session to make the SessionID static
                            this.Session["UserName"] = username;

                            user.UpdateSession(this.Session.SessionID);
                        }
                        return new FormsLoginResult(username);
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
            return View();
        }

        /// <summary>
        /// Logouts the currently logged in user.
        /// </summary>
        /// <returns>A redirect to the home page</returns>
        public ActionResult Logout()
        {
            return new FormsLogoutResult();
        }
        
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <returns>Returns the Register view</returns>
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Registers the specified username.
        /// </summary>
        /// <param name="username">The username to register.</param>
        /// <param name="email">The email for the new user.</param>
        /// <param name="password">The password.</param>
        /// <param name="confirmPassword">The confirm password.</param>
        /// <returns>The Register view on error, redirects to SendVerificationCode on success.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Register(string username, string email, string password, string confirmPassword)
        {
            // Basic parameter validation
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "You must specify a username.", username);
            }

            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.", email);
            }

            if (password == null || password.Length < this.Provider.MinRequiredPasswordLength)
            {
                string passwordError = String.Format(CultureInfo.CurrentCulture, "You must specify a new password of {0} or more characters.", this.Provider.MinRequiredPasswordLength);
                ModelState.AddModelError("password", passwordError, password);
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
                    Logger.Write("Failed reCAPTCHA attempt", "Controller", 200, 0, TraceEventType.Verbose, "Failed reCAPTCHA attempt", props);
                    ModelState.AddModelError("recaptcha", "reCAPTCHA failed to verify", humanValidator.Response);
                }
            }

            if (ViewData.ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                CosmoMongerMembershipUser newUser = (CosmoMongerMembershipUser)this.Provider.CreateUser(username, password, email, null, null, false, null, out createStatus);

                if (newUser != null)
                {
                    return RedirectToAction("SendVerificationCode", new RouteValueDictionary(new { username = username }));
                }
                else
                {
                    ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        /// <summary>
        /// Sends the verification code to the users email.
        /// </summary>
        /// <returns>The SendVerificationCode view on error, redirects to SendVerificationCodeSuccess on success.</returns>
        /*public ActionResult SendVerificationCode()
        {
            return View();
        }*/

        /// <summary>
        /// Sends the verification code to the users email.
        /// </summary>
        /// <param name="username">The username to send the verification code for.</param>
        /// <returns>The SendVerificationCode view on error, redirects to SendVerificationCodeSuccess on success.</returns>
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendVerificationCode(string username)
        {
            if (!String.IsNullOrEmpty(username))
            {
                CosmoMongerMembershipUser verifyUser = (CosmoMongerMembershipUser)this.Provider.GetUser(username, false);
                if (verifyUser != null)
                {
                    string baseVerificationUrl = this.Request.Url.GetLeftPart(UriPartial.Authority) + this.Url.Action("VerifyEmail") + "?username=" + this.Url.Encode(username) + "&verificationCode=";
                    try
                    {
                        verifyUser.SendVerificationCode(baseVerificationUrl);
                        return RedirectToAction("SendVerificationCodeSuccess");
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Log this exception
                        ExceptionPolicy.HandleException(ex, "Controller Policy");

                        // Failed to send e-mail
                        ModelState.AddModelError("_FORM", ex.Message);
                    }
                }
                else
                {
                    // Username is invalid
                    ModelState.AddModelError("username", "Invalid username", username);
                }
            }

            // If we got this far, something failed
            return View();
        }

        /// <summary>
        /// Shows the SendVerificationCodeSuccess view.
        /// </summary>
        /// <returns>Returns the SendVerificationCodeSuccess View</returns>
        public ActionResult SendVerificationCodeSuccess()
        {
            return View();
        }

        /// <summary>
        /// Verifies the supplied email address for the supplied username.
        /// </summary>
        /// <param name="username">The username to verify the email of.</param>
        /// <param name="verificationCode">The verification code.</param>
        /// <returns>The VerifyEmail View on error, redirects to the VerifyEmailSuccess action if successful.</returns>
        public ActionResult VerifyEmail(string username, string verificationCode)
        {
            CosmoMongerMembershipUser checkUser = (CosmoMongerMembershipUser)this.Provider.GetUser(username, false);
            if (checkUser != null)
            {
                if (checkUser.VerifyEmail(verificationCode))
                {
                    return RedirectToAction("VerifyEmailSuccess", new RouteValueDictionary(new { email = checkUser.Email }));
                }
                else
                {
                    ModelState.AddModelError("verificationCode", "Invalid verification code", verificationCode);
                }
            }
            else
            {
                ModelState.AddModelError("username", "Invalid username", username);
            }

            return View();
        }

        /// <summary>
        /// Shows the VerifyEmailSuccess view.
        /// </summary>
        /// <param name="email">The email that was successfully verified.</param>
        /// <returns>Returns the VerifyEmailSuccess View.</returns>
        public ActionResult VerifyEmailSuccess(string email)
        {
            ViewData["Email"] = email;
            return View();
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <returns>The ChangePassword view</returns>
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="currentPassword">The current password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="confirmPassword">The confirm password.</param>
        /// <returns>The ChangePassword view on error, redirects to the ChangePasswordSuccess action on success.</returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            // Basic parameter validation
            if (String.IsNullOrEmpty(currentPassword))
            {
                ModelState.AddModelError("currentPassword", "You must specify a current password.", currentPassword);
            }

            if (newPassword == null || newPassword.Length < this.Provider.MinRequiredPasswordLength)
            {
                string passwordError = String.Format(CultureInfo.CurrentCulture, "You must specify a new password of {0} or more characters.", this.Provider.MinRequiredPasswordLength);
                ModelState.AddModelError("newPassword", passwordError, newPassword);
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
                    changeSuccessful = this.Provider.ChangePassword(User.Identity.Name, currentPassword, newPassword);
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

        /// <summary>
        /// Shows the password change success view
        /// </summary>
        /// <returns>The ChangePasswordSuccess view</returns>
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        /// <summary>
        /// Updates the users e-mail with the newly supplied e-mail
        /// </summary>
        /// <returns>
        /// The UpdateEmail() action results.
        /// </returns>
        [Authorize]
        public ActionResult ChangeEmail()
        {
            MembershipUser user = this.Provider.GetUser(User.Identity.Name, true);
            ViewData["Email"] = user.Email;

            return View();
        }

        /// <summary>
        /// Updates the users email with the newly supplied e-mail
        /// </summary>
        /// <param name="email">The updated email to use.</param>
        /// <returns>
        /// The ChangeEmail view on error, a redirect to the ChangeEmailSuccess action otherwise.
        /// </returns>
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeEmail(string email)
        {
            // Basic parameter validation
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify a new email.", email);
            }

            if (ModelState.IsValid)
            {
                // Attempt to change email
                try
                {
                    MembershipUser user = this.Provider.GetUser(User.Identity.Name, true);
                    user.Email = email;

                    return RedirectToAction("ChangeEmailSuccess");
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    // Display error
                    ModelState.AddModelError("email", ex.Message, email);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        /// <summary>
        /// Returns the changed email success view
        /// </summary>
        /// <returns>The ChangeEmailSuccess View</returns>
        public ActionResult ChangeEmailSuccess()
        {
            return View();
        }

        /// <summary>
        /// This action returns the users email and username to the UserProfile view.
        /// </summary>
        /// <returns>The UserProfile view</returns>
        public ActionResult UserProfile()
        {
            CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.Provider.GetUser(User.Identity.Name, true);
            if (user != null)
            {
                ViewData["Email"] = user.Email;
                ViewData["Name"] = user.UserName;
                User userModel = user.GetUserModel();
                ViewData["JoinDate"] = userModel.Joined;

                return View();
            }

            // Ran into an error, redirect to login
            return RedirectToAction("Login");
        }

        /// <summary>
        /// The action returns the ForgotPassword view so that a user can enter their e-mail 
        /// and start the forgotten password process.
        /// </summary>
        /// <returns>The ForgotPassword view</returns>
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>The ForgotPassword view on error, redirect to the ForgotPasswordSuccess action on success.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForgotPassword(string email)
        {
            string username = String.Empty;

            // Basic parameter validation
            if (String.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "You must specify an email address.", email);
            }
            else
            {
                username = this.Provider.GetUserNameByEmail(email);
                if (String.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError("email", "No user registered with that email address.", email);
                }
            }

            if (ModelState.IsValid)
            {
                // Attempt to sent password reset email
                try
                {
                    CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.Provider.GetUser(username, false);
                    string basePasswordResetUrl = this.Request.Url.GetLeftPart(UriPartial.Authority) + this.Url.Action("ResetPassword") + "?username=" + this.Url.Encode(username) + "&resetPasswordCode=";
                    if (user != null)
                    {
                        user.SendForgotPasswordLink(basePasswordResetUrl);

                        return RedirectToAction("ForgotPasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("email", "Unable to lookup user with matching email address.", email);
                    }
                }
                catch (ArgumentException ex)
                {
                    // Log this exception
                    ExceptionPolicy.HandleException(ex, "Controller Policy");

                    // Display error
                    ModelState.AddModelError("email", ex.Message, email);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        /// <summary>
        /// Returns the ForgotPasswordSuccess view
        /// </summary>
        /// <returns>The ForgotPasswordSuccess view</returns>
        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="resetPasswordCode">The reset password code.</param>
        /// <returns>The ResetPassword view when username/resetPasswordCode are valid. ResetPasswordError view when either is invalid.</returns>
        public ActionResult ResetPassword(string username, string resetPasswordCode)
        {
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Invalid username.", username);
            }
            if (String.IsNullOrEmpty(resetPasswordCode))
            {
                ModelState.AddModelError("resetPasswordCode", "Invalid Password Reset Code.", resetPasswordCode);
            }

            if (ModelState.IsValid)
            {
                CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.Provider.GetUser(username, false);
                if (user != null)
                {
                    if (user.CheckResetPasswordCode(resetPasswordCode))
                    {
                        // Display the username
                        ViewData["username"] = username;

                        return View();
                    }
                    else
                    {
                        ModelState.AddModelError("resetPasswordCode", "Invalid/Expired Password Reset Code.", resetPasswordCode);
                    }
                }
                else
                {
                    ModelState.AddModelError("username", "Invalid username.", username);
                }
            }

            return View("ResetPasswordError");
        }

        /// <summary>
        /// Resets the password.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="resetPasswordCode">The reset password code.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>The ResetPasswordSuccess view when the password is successfully reset. ResetPasswordError view when either is invalid.</returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResetPassword(string username, string resetPasswordCode, string newPassword)
        {
            if (String.IsNullOrEmpty(username))
            {
                ModelState.AddModelError("username", "Invalid username.", username);
            }
            if (String.IsNullOrEmpty(resetPasswordCode))
            {
                ModelState.AddModelError("resetPasswordCode", "Invalid Password Reset Code.", resetPasswordCode);
            }

            if (ModelState.IsValid)
            {
                CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.Provider.GetUser(username, false);
                if (user != null)
                {
                    if (user.CheckResetPasswordCode(resetPasswordCode))
                    {
                        newPassword = user.ResetPassword(resetPasswordCode);
                        if (!String.IsNullOrEmpty(newPassword))
                        {
                            // Clear out the reset password code so it cannot be used again
                            user.ClearResetPasswordCode();

                            // Display username
                            ViewData["username"] = username;
                            // Display new password
                            ViewData["newPassword"] = newPassword;

                            return View("ResetPasswordSuccess");
                        }
                        else
                        {
                            ModelState.AddModelError("resetPasswordCode", "Unable to reset password.", resetPasswordCode);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("resetPasswordCode", "Invalid/Expired Password Reset Code.", resetPasswordCode);
                    }
                }
                else
                {
                    ModelState.AddModelError("username", "Invalid username.", username);
                }
            }

            return View("ResetPasswordError");
        }

        /// <summary>
        /// Converts an MembershipCreateStatus error code to a friendly string.
        /// </summary>
        /// <param name="createStatus">The create status error to convert.</param>
        /// <returns>An user friendly string</returns>
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
}
