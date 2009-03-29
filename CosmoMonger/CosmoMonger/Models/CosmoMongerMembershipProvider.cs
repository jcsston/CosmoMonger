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
        /// Gets or sets the name of the application using the custom membership provider.
        /// The set is not implemented and will throw a NotImplementedException if an assignment is attempted.
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
        /// Gets a value indicating whether the membership provider is configured to allow users to reset their passwords.
        /// </summary>
        /// <value>Is always true.</value>
        /// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.</returns>
        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to allow users to retrieve their passwords.
        /// </summary>
        /// <value>Is always false.</value>
        /// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.</returns>
        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
        /// </summary>
        /// <value>Is always 3.</value>
        /// <returns>The number of invalid password or password-answer attempts allowed before the membership user is locked out.</returns>
        public override int MaxInvalidPasswordAttempts
        {
            get { return 3; }
        }

        /// <summary>
        /// Gets the minimum number of special characters that must be present in a valid password.
        /// </summary>
        /// <value>Is always 2.</value>
        /// <returns>The minimum number of special characters that must be present in a valid password.</returns>
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets the minimum length required for a password.
        /// </summary>
        /// <value>Is always 6.</value>
        /// <returns>The minimum length required for a password. </returns>
        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        /// <summary>
        /// Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.
        /// </summary>
        /// <value>Is always 15.</value>
        /// <returns>The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.</returns>
        public override int PasswordAttemptWindow
        {
            get { return 15; }
        }

        /// <summary>
        /// Gets a value indicating the format for storing passwords in the membership data store.
        /// </summary>
        /// <value>Is always Hashed.</value>
        /// <returns>One of the <see cref="T:System.Web.Security.MembershipPasswordFormat"/> values indicating the format for storing passwords in the data store.</returns>
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        /// <summary>
        /// Gets the regular expression used to evaluate a password.
        /// Not implemented.
        /// </summary>
        /// <value>Always throws a NotImplementedException exception.</value>
        /// <returns>
        /// A regular expression used to evaluate a password.
        /// </returns>
        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.
        /// </summary>
        /// <value>Always is false.</value>
        /// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.</returns>
        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
        /// </summary>
        /// <value>Always is true.</value>
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
            CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.GetUser(username, true);
            if (user != null)
            {
                return user.ValidatePassword(password)
                    && user.IsApproved 
                    && !user.IsLockedOut;
            }

            return false;
        }

        /// <summary>
        /// Removes a user from the membership data source.
        /// </summary>
        /// <param name="username">The name of the user to delete.</param>
        /// <param name="deleteAllRelatedData">Must be true. Only supports deleting all user data from database.</param>
        /// <exception cref="ArgumentException">An ArgumentException for deleteAllRelatedData is thrown if deleteAllRelatedData is false.</exception>
        /// <returns>
        /// true if the user was successfully deleted; otherwise, false.
        /// </returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (!deleteAllRelatedData)
            {
                throw new ArgumentException("Must delete all related data to user", "deleteAllRelatedData");
            }

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.UserName == username
                                 select u).SingleOrDefault();
            if (matchingUser != null)
            {
                ////db.Log = Console.Out;

                // We have to delete all objects in the database related to this user
                var playerShips = (from p in db.Players where p.User == matchingUser select p.Ship);
                var playerShipIds = (from p in db.Players where p.User == matchingUser select p.Ship.ShipId);
                db.PlayerRecords.DeleteAllOnSubmit(from r in db.PlayerRecords where r.Player.User == matchingUser select r);
                db.Players.DeleteAllOnSubmit(from p in db.Players where p.User == matchingUser select p);
                List<int> playerCombatIds = (from c in db.Combats
                                             where playerShipIds.Contains(c.AttackerShipId)
                                             || playerShipIds.Contains(c.DefenderShipId)
                                             select c.CombatId).ToList();
                db.CombatGoods.DeleteAllOnSubmit(from g in db.CombatGoods where playerCombatIds.Contains(g.CombatId) select g);
                db.Combats.DeleteAllOnSubmit(from c in db.Combats where playerCombatIds.Contains(c.CombatId) select c);
                db.ShipGoods.DeleteAllOnSubmit(from g in db.ShipGoods where playerShips.Contains(g.Ship) select g);
                db.Ships.DeleteAllOnSubmit(playerShips);
                db.BuddyLists.DeleteAllOnSubmit(from b in db.BuddyLists
                                              where b.FriendId == matchingUser.UserId
                                              || b.UserId == matchingUser.UserId
                                              select b);
                db.IgnoreLists.DeleteAllOnSubmit(from i in db.IgnoreLists
                                                where i.AntiFriendId == matchingUser.UserId
                                                || i.UserId == matchingUser.UserId
                                                select i);
                db.Messages.DeleteAllOnSubmit(from m in db.Messages
                                              where m.RecipientUserId == matchingUser.UserId
                                              || m.SenderUserId == matchingUser.UserId
                                              select m);
                db.Users.DeleteOnSubmit(matchingUser);
                db.SubmitChanges();
                
                ////db.Log = null;

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
            CosmoMongerMembershipUser user = (CosmoMongerMembershipUser)this.GetUser(username, true);
            if (user != null)
            {
                return user.ChangePassword(oldPassword, newPassword);
            }

            return false;
        }

        /// <summary>
        /// Not implemented.
        /// Processes a request to update the password question and answer for a membership user.
        /// </summary>
        /// <param name="username">The user to change the password question and answer for.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <param name="newPasswordQuestion">The new password question for the specified user.</param>
        /// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
        /// <returns>
        /// true if the password question and answer are updated successfully; otherwise, false.
        /// </returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.
        /// </summary>
        /// <param name="emailToMatch">The e-mail address to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            var matchingUsers = (from u in db.Users
                                 where u.Email.Contains(emailToMatch)
                                 select u);
            return this.FindUsersBy(matchingUsers, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of membership users where the user name contains the specified user name to match.
        /// </summary>
        /// <param name="usernameToMatch">The user name to search for.</param>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            var matchingUsers = (from u in db.Users
                                 where u.UserName.Contains(usernameToMatch)
                                 select u);
            return this.FindUsersBy(matchingUsers, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. <paramref name="pageIndex"/> is zero-based.</param>
        /// <param name="pageSize">The size of the page of results to return.</param>
        /// <param name="totalRecords">The total number of matched users.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUserCollection"/> collection that contains a page of <paramref name="pageSize"/><see cref="T:System.Web.Security.MembershipUser"/> objects beginning at the page specified by <paramref name="pageIndex"/>.
        /// </returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            var matchingUsers = (from u in db.Users select u);
            return this.FindUsersBy(matchingUsers, pageIndex, pageSize, out totalRecords);
        }

        /// <summary>
        /// Not implemented.
        /// Gets the number of users currently accessing the application.
        /// </summary>
        /// <returns>
        /// The number of users currently accessing the application.
        /// </returns>
        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not implemented.
        /// Gets the password for the specified user name from the data source.
        /// </summary>
        /// <param name="username">The user to retrieve the password for.</param>
        /// <param name="answer">The password answer for the user.</param>
        /// <returns>
        /// The password for the specified user name.
        /// </returns>
        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the user associated with the specified username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// The user associated with the specified username
        /// If no match is found, return null.
        /// </returns>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.UserName == username
                                 select u).SingleOrDefault();
            if (matchingUser != null)
            {
                if (userIsOnline)
                {
                    matchingUser.LastActivity = DateTime.UtcNow;
                }

                return new CosmoMongerMembershipUser(matchingUser);
            }

            return null;
        }

        /// <summary>
        /// Not Implemented.
        /// Gets user information from the data source based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.
        /// </summary>
        /// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
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
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.Email == email
                                 select u).SingleOrDefault();
            if (matchingUser != null)
            {
                return matchingUser.UserName;
            }

            return String.Empty;
        }

        /// <summary>
        /// Not Implemented.
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <param name="username">The user to reset the password for.</param>
        /// <param name="answer">The password answer for the specified user.</param>
        /// <returns>The new password for the specified user.</returns>
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

        /// <summary>
        /// Not Implemented.
        /// Updates information about a user in the data source.
        /// </summary>
        /// <param name="user">A <see cref="T:System.Web.Security.MembershipUser"/> object that represents the user to update and the updated information for the user.</param>
        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Helper function to build a MembershipUserCollection for a matching users query result.
        /// </summary>
        /// <param name="matchingUsers">The matching users query to build the collection from.</param>
        /// <param name="pageIndex">Index of the page of the collection to return.</param>
        /// <param name="pageSize">Size of each page in the collection.</param>
        /// <param name="totalRecords">Set to the total number records in the collection.</param>
        /// <returns>A new MembershipUserCollection containing the contents of the 'page' selected.</returns>
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
    }
}
