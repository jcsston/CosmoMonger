//-----------------------------------------------------------------------
// <copyright file="CosmoMongerMembershipProvider.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
	using System.Text;
    using System.Web;
    using System.Web.Security;
	using System.Security.Cryptography;

    /// <summary>
    /// This is an implementation of the System.Web.Security.MembershipProvider class and will be used by the AccountController
    /// </summary>
    public class CosmoMongerMembershipProvider : MembershipProvider
    {
        public override string ApplicationName
        {
            get
            {
                return "CosmoMonger";
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 2; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 15; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

		private byte [] HashPassword(string password)
		{
			// We need to convert the password string to a byte string for encoding
			byte[] passwordBytes = Encoding.Default.GetBytes(password);
			SHA1 hasher = SHA1.Create();
			return hasher.ComputeHash(passwordBytes);
		}

		private MembershipUser DatabaseUserToMembershipUser(User u)
		{
			return new MembershipUser("CosmoMongerMembershipProvider", u.UserName, null, u.Email, null, null, u.Validated, u.Active, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
		}

		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			CosmoMongerDbDataContext db = new CosmoMongerDbDataContext();

			// Check for an existing user
			bool matchingUsername = (from u in db.Users where u.UserName == username select u).Any();
			if (matchingUsername)
			{
				status = MembershipCreateStatus.DuplicateUserName;
				return null;
			}
			bool matchingEmail = (from u in db.Users where u.Email == email select u).Any();
			if (matchingEmail)
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			// Create the new user
			User user = new User();
			user.UserName = username;
			user.Email = email;
			user.Password = HashPassword(password);
			user.Active = isApproved;

			// Insert the user record into the database
			db.Users.InsertOnSubmit(user);
			db.SubmitChanges();

			status = MembershipCreateStatus.Success;

			return DatabaseUserToMembershipUser(user);
		}

		public override bool ValidateUser(string username, string password)
		{
			CosmoMongerDbDataContext db = new CosmoMongerDbDataContext();
			bool validLogin = (from u in db.Users 
							   where u.UserName == username 
							   && u.Password == HashPassword(password) 
							   && u.Validated && u.Active select u).Any();
			return validLogin;
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			throw new NotImplementedException();
		}

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
			CosmoMongerDbDataContext db = new CosmoMongerDbDataContext();
			// Find the user in the database and verify the current password
			User currentUser = (from u in db.Users
							   where u.UserName == username
							   && u.Password == HashPassword(oldPassword)
							   select u).First();
			if (currentUser != null)
			{
				// Update the users password
				currentUser.Password = HashPassword(newPassword);
				db.SubmitChanges();
				return true;
			}
			return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
			throw new NotImplementedException();
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
    }
}
