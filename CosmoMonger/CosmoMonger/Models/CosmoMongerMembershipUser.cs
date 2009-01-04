//-----------------------------------------------------------------------
// <copyright file="CosmoMongerMembershipUser.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using System.Security.Cryptography;
    using System.Diagnostics;
    using System.Text;
    using System.Net.Mail;

    public class CosmoMongerMembershipUser : MembershipUser
    {
        private User user = null;

        //public override string Comment { get; set; }
        //public override DateTime CreationDate { get; }

        public override string Email 
        {
            get
            {
                return this.user.Email;
            }
            set
            {
                this.user.UpdateProfile(this.user.UserName, this.user.Email);
            }
        }

        public override bool IsApproved
        {
            get
            {
                return this.user.Active;
            }
            set
            {
                this.user.Active = value;
            }
        }

        public override bool IsLockedOut
        {
            get
            {
                return !this.IsApproved;
            }
        }
        
        //public override DateTime LastActivityDate { get; set; }
        //public override DateTime LastLockoutDate { get; }
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

        //public override DateTime LastPasswordChangedDate { get; }
        public override string ProviderName 
        {
            get
            {
                return "CosmoMongerMembershipProvider";
            }
        }

        public override string UserName
        {
            get
            {
                return this.user.UserName;
            }
        }

        public string VerificationCode
        {
            get
            {
                string code = this.user.UserId.ToString();
                return code;
            }
        }

        public CosmoMongerMembershipUser(User u)
        {
            this.user = u;
        }

        /// <summary>
        /// Hashes the password using SHA1
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>byte array containing the SHA1 hash.</returns>
        static private byte[] HashPassword(string password)
        {
            Logger.Write("Hashing Password: " + password, "Business Object", 1, 1000, TraceEventType.Verbose);
            // We need to convert the password string to a byte string for encoding
            byte[] passwordBytes = Encoding.UTF8.GetBytes("CosmoMonger+" + password + "Rules!");
            SHA512 hasher = SHA512.Create();
            // Hash the password
            passwordBytes = hasher.ComputeHash(passwordBytes);
            Logger.Write("Hashed Password to: " + passwordBytes.ToString(), "Business Object", 1, 1001, TraceEventType.Verbose);
            return passwordBytes;
        }

        static public CosmoMongerMembershipUser CreateUser(string username, string password, string email)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

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
            user.Password = HashPassword(password);
            user.Active = true;
            // TODO: Add e-mail validatation
            user.Validated = true;

            // Insert the user record into the database
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            return new CosmoMongerMembershipUser(user);
        }

        /// <summary>
        /// Change the password for a user
        /// </summary>
        /// <param name="username">The user to update the password for.</param>
        /// <param name="oldPassword">The current password for the specified user.</param>
        /// <param name="newPassword">The new password for the specified user.</param>
        /// <returns>
        /// true if the password was updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePassword(string oldPassword, string newPassword)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            if (user != null && ValidatePassword(oldPassword))
            {
                // Update the users password
                user.Password = HashPassword(newPassword);
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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            bool validPassword = true;
            byte [] hashedPassword = HashPassword(password);
            byte[] correctPassword = this.user.Password;

            Debug.Assert(hashedPassword.Length == correctPassword.Length, "SHA1 Hashed Passwords should always be the same length");
            
            for (int i = 0; i < hashedPassword.Length; i++)
            {
                if (hashedPassword[i] != correctPassword[i])
                {
                    validPassword = false;
                    break;
                }
            }

            if (validPassword)
            {
                this.user.LoginAttemptCount = 0;
                this.user.LastLogin = DateTime.Now;
                db.SubmitChanges();
                return true;
            }
            else
            {
                this.user.LoginAttemptCount += 1;
                // Disable the account if login attempts pass 5 times
                if (this.user.LoginAttemptCount > 5)
                {
                    this.user.Active = false;
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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

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
        ///  Clears the locked-out state of the user so that the user can login.
        /// </summary>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        public override bool UnlockUser()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            if (this.user != null)
            {
                this.user.Active = true;
                db.SubmitChanges();
                return true;
            }
            return false;
        }

        public User GetUserModel()
        {
            return this.user;
        }

        public bool SendVerificationCode(string baseVerificationCodeUrl)
        {
            // Build e-mail message
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("admin@cosmomonger.com", "CosmoMonger");
            msg.To.Add(this.Email);
            msg.Subject = "Email Verification for CosmoMonger";
            msg.Body =
                "To verify your CosmoMonger account go to\n" +
                baseVerificationCodeUrl + this.VerificationCode + "\n\n" +
                "Verification Code: " + this.VerificationCode;

            // Send e-mail
            SmtpClient smtp = new SmtpClient();
            smtp.Send(msg);

            return true;
        }
    }
}
