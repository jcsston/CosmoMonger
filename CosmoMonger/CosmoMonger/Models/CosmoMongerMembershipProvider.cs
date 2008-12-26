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
    using System.Diagnostics;
    using System.Linq;
	using System.Text;
    using System.Web;
    using System.Web.Security;

    /// <summary>
    /// This is an implementation of the System.Web.Security.MembershipProvider class and will be used by the AccountController
    /// Nanages user creation and membership
    /// </summary>
    public class CosmoMongerMembershipProvider : MembershipProvider
    {
		/// <summary>
		/// Gets the name of the application using the custom membership provider.
		/// </summary>
		/// <value>The string CosmoMonger</value>
		/// <returns>The name of the application using the custom membership provider.</returns>
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

		/// <summary>
		/// Indicates whether the membership provider is configured to allow users to reset their passwords.
		/// </summary>
		/// <value>true</value>
		/// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.</returns>
        public override bool EnablePasswordReset
        {
            get { return true; }
        }

		/// <summary>
		/// Indicates whether the membership provider is configured to allow users to retrieve their passwords.
		/// </summary>
		/// <value>false</value>
		/// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.</returns>
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

		/// <summary>
		/// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
		/// </summary>
		/// <value>3</value>
		/// <returns>The number of invalid password or password-answer attempts allowed before the membership user is locked out.</returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

		/// <summary>
		/// Gets the minimum number of special characters that must be present in a valid password.
		/// </summary>
		/// <value>2</value>
		/// <returns>The minimum number of special characters that must be present in a valid password.</returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 2; }
        }

		/// <summary>
		/// Gets the minimum length required for a password.
		/// </summary>
		/// <value>6</value>
		/// <returns>The minimum length required for a password. </returns>
        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

		/// <summary>
		/// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
		/// </summary>
		/// <value>15</value>
		/// <returns>The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.</returns>
        public override int PasswordAttemptWindow
        {
            get { return 15; }
        }

		/// <summary>
		/// Gets a value indicating the format for storing passwords in the membership data store.
		/// </summary>
		/// <value>Hashed</value>
		/// <returns>One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.</returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

		/// <summary>
		/// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
		/// </summary>
		/// <value>false</value>
		/// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.</returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

		/// <summary>
		/// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
		/// </summary>
		/// <value>true</value>
		/// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.</returns>
        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

		/// <summary>
		/// Adds a new membership user to the data source.
		/// </summary>
		/// <param name="username">The user name for the new user.</param>
		/// <param name="password">The password for the new user.</param>
		/// <param name="email">The e-mail address for the new user.</param>
		/// <param name="passwordQuestion">The password question for the new user.</param>
		/// <param name="passwordAnswer">The password answer for the new user</param>
		/// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
		/// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
		/// <param name="status">A <see cref="T:System.Web.Security.MembershipCreateStatus"/> enumeration value indicating whether the user was created successfully.</param>
		/// <returns>
		/// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the information for the newly created user.
		/// </returns>
		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
            try
            {
                // Create the new user
                CosmoMongerMembershipUser user = CosmoMongerMembershipUser.CreateUser(username, password, email);

                status = MembershipCreateStatus.Success;
                return user;
            }
            catch (ArgumentException ex)
            {
                if (ex.ParamName == "username")
                {
                    status = MembershipCreateStatus.DuplicateUserName;
                }
                else if (ex.ParamName == "email")
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                }
                else if (ex.ParamName == "password")
                {
                    status = MembershipCreateStatus.InvalidPassword;
                }
                else
                {
                    // We don't know how to handle this error
                    status = MembershipCreateStatus.ProviderError;
                }
                return null;
            }
		}

		/// <summary>
		/// Verifies that the specified user name and password exist in the CosmoMonger database and that the user is active and validated.
		/// </summary>
		/// <param name="username">The name of the user to validate.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <returns>
		/// true if the specified username and password are valid; otherwise, false.
		/// </returns>
		public override bool ValidateUser(string username, string password)
		{
            CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.GetUser(username, false);
            if (user != null)
            {
                return user.ValidatePassword(password)
                    && user.IsApproved 
                    && !user.IsLockedOut;
            }
            return false;
		}

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
            if (!deleteAllRelatedData)
            {
                throw new ArgumentException("Must delete all related data to user", "deleteAllRelatedData");
            }

            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.UserName == username
                                 select u).SingleOrDefault();
            if (matchingUser != null)
            {
                // We have to delete all objects in the database related to this user
                var playerShips = (from p in db.Players where p.User == matchingUser select p.Ship);
                var playerShipIds = (from p in db.Players where p.User == matchingUser select p.Ship.ShipId);
                db.InProgressCombats.DeleteAllOnSubmit(from c in db.InProgressCombats
                                                       where playerShipIds.Contains(c.AttackerShipId)
                                                       || playerShipIds.Contains(c.DefenderShipId)
                                                       select c);
                db.ShipGoods.DeleteAllOnSubmit(from g in db.ShipGoods where playerShips.Contains(g.Ship) select g);
                db.Ships.DeleteAllOnSubmit(playerShips);
                db.Players.DeleteAllOnSubmit(from p in db.Players where p.User == matchingUser select p);
                db.Messages.DeleteAllOnSubmit(from m in db.Messages
                                              where m.RecipientUserId == matchingUser.UserId
                                              || m.SenderUserId == matchingUser.UserId
                                              select m);
                db.Users.DeleteOnSubmit(matchingUser);
                db.SubmitChanges();
                return true;
            }
            return false;
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
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.GetUser(username, false);
            if (user != null)
            {
                return user.ChangePassword(oldPassword, newPassword);
            }
            return false;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        private MembershipUserCollection FindUsersBy(IEnumerable<User> matchingUsers, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = matchingUsers.Count();
            matchingUsers = matchingUsers.Skip(pageIndex * pageSize).Take(pageSize);

            MembershipUserCollection col = new MembershipUserCollection();
            foreach (User user in matchingUsers)
            {
                col.Add(new CosmoMongerMembershipUser(user));
            }

            return col;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            var matchingUsers = (from u in db.Users
                                 where u.Email.Contains(emailToMatch)
                                 select u);
            return FindUsersBy(matchingUsers, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            var matchingUsers = (from u in db.Users
                                 where u.UserName.Contains(usernameToMatch)
                                 select u);
            return FindUsersBy(matchingUsers, pageIndex, pageSize, out totalRecords);
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            var matchingUsers = (from u in db.Users select u);
            return FindUsersBy(matchingUsers, pageIndex, pageSize, out totalRecords);
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the user associated with the specified username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>
        /// The user associated with the specified username 
        /// If no match is found, return null.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.UserName == username
                                 select u).SingleOrDefault();
            if (matchingUser != null)
            {
                return new CosmoMongerMembershipUser(matchingUser);
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Gets the user name associated with the specified e-mail address.
		/// </summary>
		/// <param name="email">The e-mail address to search for.</param>
		/// <returns>
		/// The user name associated with the specified e-mail address. 
        /// If no match is found, return empty string.
		/// </returns>
        public override string GetUserNameByEmail(string email)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.Email == email
                                 select u).SingleOrDefault();
			if (matchingUser != null)
			{
				return matchingUser.UserName;
			}

			return "";
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

		/// <summary>
		/// Clears a lock on a user so that the user can login.
		/// </summary>
		/// <param name="userName">The membership user whose lock status you want to clear.</param>
		/// <returns>
		/// true if the membership user was successfully unlocked; otherwise, false.
		/// </returns>
        public override bool UnlockUser(string userName)
        {
            CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.GetUser(userName, false);
            if (user != null)
            {
                return user.UnlockUser();
            }
            return false;
			
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }
    }
}
