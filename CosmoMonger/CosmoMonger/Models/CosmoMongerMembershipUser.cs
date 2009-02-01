//-----------------------------------------------------------------------
// <copyright file="CosmoMongerMembershipUser.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Mail;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    /// <summary>
    /// This class represents a user account in the system and manages checking the password,
    /// changing the password, and sending verification e-mails.
    /// </summary>
    public class CosmoMongerMembershipUser : MembershipUser
    {
        /// <summary>
        /// Stores a reference to the backing User model object.
        /// </summary>
        private User user = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="CosmoMongerMembershipUser"/> class.
        /// </summary>
        /// <param name="u">The User model object for this user.</param>
        public CosmoMongerMembershipUser(User u)
        {
            this.user = u;
        }

        ////public override string Comment { get; set; }
        ////public override DateTime CreationDate { get; }

        /// <summary>
        /// Gets or sets the e-mail address for the membership user.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The e-mail address for the membership user.
        /// </returns>
        public override string Email 
        {
            get
            {
                return this.user.Email;
            }

            set
            {
                this.user.UpdateEmail(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the membership user can be authenticated.
        /// </summary>
        /// <value></value>
        /// <returns>true if the user can be authenticated; otherwise, false.
        /// </returns>
        public override bool IsApproved
        {
            get
            {
                return this.user.Validated;
            }

            set
            {
                this.user.Validated = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the membership user is locked out and unable to login.
        /// </summary>
        /// <value></value>
        /// <returns>true if the membership user is locked out and unable to login; otherwise, false.
        /// </returns>
        public override bool IsLockedOut
        {
            get
            {
                return !this.user.Active;
            }
        }

        /// <summary>
        /// Gets or sets the date and time when the membership user was last authenticated or accessed the application.
        /// If the user has never accessed the application, DateTime.MinValue is returned.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The date and time when the membership user was last authenticated or accessed the application.
        /// </returns>
        public override DateTime LastActivityDate 
        {
            get
            {
                return this.user.LastActivity.GetValueOrDefault(DateTime.MinValue);
            }

            set
            {
                this.user.LastActivity = value;
            }
        }

        ////public override DateTime LastLockoutDate { get; }

        /// <summary>
        /// Gets or sets the date and time when the user was last authenticated.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The date and time when the user was last authenticated.
        /// </returns>
        public override DateTime LastLoginDate 
        {
            get
            {
                return this.user.LastLogin.GetValueOrDefault(DateTime.Now);
            }

            set
            {
                this.user.LastLogin = value;
            }
        }

        ////public override DateTime LastPasswordChangedDate { get; }

        /// <summary>
        /// Gets the name of the membership provider that stores and retrieves user information for the membership user.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The name of the membership provider that stores and retrieves user information for the membership user.
        /// </returns>
        public override string ProviderName 
        {
            get
            {
                return "CosmoMongerMembershipProvider";
            }
        }

        /// <summary>
        /// Gets the logon name of the membership user.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The logon name of the membership user.
        /// </returns>
        public override string UserName
        {
            get
            {
                return this.user.UserName;
            }
        }

        /// <summary>
        /// Gets the verification code sent to the users e-mail address to verify their e-mail.
        /// </summary>
        /// <value>The verification code.</value>
        public string VerificationCode
        {
            get
            {
                string code = this.user.UserId.ToString();
                return code;
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="username">The username of the new user.</param>
        /// <param name="password">The password for the new user.</param>
        /// <param name="email">The email address for the new user.</param>
        /// <exception cref="ArgumentException">
        /// Throws an ArgumentException for the username param if there is another user 
        /// already in the system with the same username.
        /// Throws an ArgumentException for the email param if there is another user
        /// already in the system with the same e-mail.
        /// </exception>
        /// <returns>A new CosmoMongerMembershipUser object for the newly created user.</returns>
        public static CosmoMongerMembershipUser CreateUser(string username, string password, string email)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Check for an existing user
            bool matchingUsername = (from u in db.Users where u.UserName == username select u).Any();
            if (matchingUsername)
            {
                throw new ArgumentException("Duplicate username", "username");
            }

            bool matchingEmail = (from u in db.Users where u.Email == email select u).Any();
            if (matchingEmail)
            {
                throw new ArgumentException("Duplicate email", "email");
            }

            // Create the new user
            User user = new User();
            user.UserName = username;
            user.Email = email;
            user.Password = Cryptographer.CreateHash("SHA512", password);
            user.Active = true;
            user.Validated = false;
            user.Joined = DateTime.Now;

            // Insert the user record into the database
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            return new CosmoMongerMembershipUser(user);
        }

        /// <summary>
        /// Change the password for a user
        /// </summary>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            if (this.user != null && this.ValidatePassword(oldPassword))
            {
                // Update the users password
                this.user.Password = Cryptographer.CreateHash("SHA512", newPassword);
                db.SubmitChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Verifies that the specified password matches this users password.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>
        /// true if the specified password are valid; otherwise, false.
        /// </returns>
        public bool ValidatePassword(string password)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            
            bool validPassword = Cryptographer.CompareHash("SHA512", password, this.user.Password);
            if (validPassword && this.IsApproved && !this.IsLockedOut)
            {
                this.user.LoginAttemptCount = 0;
                this.user.LastLogin = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            else
            {
                if (!this.user.Active)
                {
                    throw new InvalidOperationException("The user has to be active to validate the password");
                }

                this.user.LoginAttemptCount += 1;
                
                // If login attempts reaches 3, we start adding a delay to the login process
                // This is to prevent brute forcing login passwords
                if (this.user.LoginAttemptCount >= 3)
                {
                    // Make the user disabled in the database right now, to prevent attacks 
                    // from simply ending the connection if the login takes too long
                    this.user.Active = false;
                    db.SubmitChanges();

                    try
                    {
                        // The delay increases for every login attempt
                        // 3rd failed login 4 sec delay
                        // 4th failed login 8 sec delay
                        // 5th failed login 16 sec delay
                        // ...
                        // 10th failed login 512 sec delay
                        Thread.Sleep(1000 * (int)Math.Pow(2, this.user.LoginAttemptCount - 1));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Dictionary<string, object> props = new Dictionary<string, object>
                        {
                            { "Error", ex },
                            { "UserId", this.user.UserId },
                            { "LoginAttemptCount", this.user.LoginAttemptCount }
                        };
                        Logger.Write("Exception when delaying login", "Business Object", 600, 0, TraceEventType.Error, "Exception in CosmoMongerMembershipUser.ValidatePassword", props);
                    }

                    // Re-enable the user
                    this.user.Active = true;
                }

                db.SubmitChanges();
                return false;
            }
        }

        /// <summary>
        /// Verifies that the verification code matches this users verification code. 
        /// Used to verify the users e-mail and approve the user account.
        /// </summary>
        /// <param name="verificationCode">The verification code to check.</param>
        /// <returns>
        /// true if the specified verification code is valid and the user has been approved; otherwise, false.
        /// </returns>
        public bool VerifyEmail(string verificationCode)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            if (this.VerificationCode == verificationCode)
            {
                // Verify the user
                this.user.Validated = true;
                db.SubmitChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the locked-out state of the user so that the user can login.
        /// </summary>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        public override bool UnlockUser()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            if (this.user != null)
            {
                this.user.Active = true;
                db.SubmitChanges();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the User model object for this user.
        /// </summary>
        /// <returns>A reference to the User model object for this user</returns>
        public User GetUserModel()
        {
            return this.user;
        }

        /// <summary>
        /// Sends the verification code to the users e-mail.
        /// </summary>
        /// <param name="baseVerificationCodeUrl">The base verification code URL. Example: http://localhost:54084/Account/VerifyEmail?username=jcsston&amp;verificationCode=</param>
        /// <exception cref="SmtpException">Thrown if sending to the SMTP server failed.</exception>
        /// <exception cref="InvalidOperationException">Thrown if not enough time has passed since the last verification e-mail for this user.</exception>
        public virtual void SendVerificationCode(string baseVerificationCodeUrl)
        {
            // Check that is has been at least 5 minutes since the last
            if (this.user.LastVerificationSent.HasValue)
            {
                TimeSpan timeSinceLastVerificationEmail = DateTime.Now - this.user.LastVerificationSent.Value;

                // It needs to be at least 5 minutes since the last verification e-mail
                if (timeSinceLastVerificationEmail.TotalMinutes < 5)
                {
                    throw new InvalidOperationException("Verification e-mails can only be sent every 5 minutes.");
                }
            }

            // Build e-mail message
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("admin@cosmomonger.com", "CosmoMonger");
            msg.To.Add(this.Email);
            msg.Subject = "Email Verification for CosmoMonger";
            msg.Body =
                "Welcome to CosmoMonger. To activate your account and verify your e-mail\n" +
                "address, please click on the following link:\n\n" +
                baseVerificationCodeUrl + this.VerificationCode + "\n\n" + 
                "If you have received this mail in error, you do not need to take any\n" +
                "action to cancel the account. The account will not be activated, and\n" +
                "you will not receive any further emails.\n\n" +
                "If clicking the link above does not work, copy and paste the URL in a\n" +
                "new browser window instead.\n\n" +
                "Thank you for playing CosmoMonger.";
            try
            {
                // Send e-mail
                SmtpClient smtp = new SmtpClient();
                smtp.Send(msg);
            }
            catch (SmtpException ex)
            {
                Dictionary<string, object> props = new Dictionary<string, object>
                {
                    { "Error", ex },
                    { "UserId", this.user.UserId },
                    { "Email", this.Email },
                    { "Message", msg }
                };
                Logger.Write("Failed to send e-mail with verification code", "Model", 700, 0, TraceEventType.Error, "SmtpException in CosmoMongerMemebershipUser.SendVerificationCode", props);

                throw new InvalidOperationException("Failed to send verification e-mail. Please try again, if the problem persists, please contact admin@cosmomonger.com.", ex);
            }

            // Update datetime of last verification e-mail sent
            this.user.LastVerificationSent = DateTime.Now;
        }
    }
}
