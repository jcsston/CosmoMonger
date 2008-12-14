namespace CosmoMonger.Models
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Security.Cryptography;
    using System.Web;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    
    /// <summary>
    /// Master class to manage user creation
    /// </summary>
    public class UserManager
    {
        /// <summary>
        /// Hashes the password using SHA1
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>byte array containing the SHA1 hash.</returns>
        private byte[] HashPassword(string password)
        {
            Logger.Write("Hashing Password: " + password, "Business Object", 1, 1000, TraceEventType.Verbose);
            // We need to convert the password string to a byte string for encoding
            byte[] passwordBytes = Encoding.Default.GetBytes("CosmoMonger" + password);
            SHA1 hasher = SHA1Managed.Create();
            // Hash the password multiple times to make brute force attacks harder
            for (int i = 0; i < 1000; i++)
            {
                passwordBytes = hasher.ComputeHash(passwordBytes);
            }
            Logger.Write("Hashed Password to: " + passwordBytes.ToString(), "Business Object", 1, 1001, TraceEventType.Verbose);
            return passwordBytes;
        }

        public User CreateUser(string username, string password, string email)
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

            return user;
        }

        /// <summary>
        /// Verifies that the specified user name and password exist in the CosmoMonger database and that the user is active and validated.
        /// </summary>
        /// <param name="username">The name of the user to validate.</param>
        /// <param name="password">The password for the specified user.</param>
        /// <returns>
        /// true if the specified username and password are valid; otherwise, false.
        /// </returns>
        public bool ValidateUser(string username, string password)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            bool validLogin = (from u in db.Users
                               where u.UserName == username
                               && u.Password == HashPassword(password)
                               && u.Validated && u.Active
                               select u).Any();
            return validLogin;
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
        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            // Find the user in the database and verify the current password
            User currentUser = (from u in db.Users
                                where u.UserName == username
                                && u.Password == HashPassword(oldPassword)
                                select u).SingleOrDefault();
            if (currentUser != null)
            {
                // Update the users password
                currentUser.Password = HashPassword(newPassword);
                db.SubmitChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the user associated with the specified e-mail address.
        /// </summary>
        /// <param name="email">The e-mail address to search for.</param>
        /// <returns>
        /// The user associated with the specified e-mail address. 
        /// If no match is found, return null.
        /// </returns>
        public User GetUserByEmail(string email)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.Email == email
                                 select u).SingleOrDefault();
            return matchingUser;
        }

        /// <summary>
        /// Gets the user associated with the specified username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>
        /// The user associated with the specified username 
        /// If no match is found, return null.
        /// </returns>
        public User GetUserByUserName(string username)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.UserName == username
                                 select u).SingleOrDefault();
            return matchingUser;
        }

        /// <summary>
        /// Clears a lock on a user so that the user can login.
        /// </summary>
        /// <param name="userName">The membership user whose lock status you want to clear.</param>
        /// <returns>
        /// true if the membership user was successfully unlocked; otherwise, false.
        /// </returns>
        public bool UnlockUser(string userName)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User matchingUser = (from u in db.Users
                                 where u.UserName == userName
                                 select u).SingleOrDefault();
            if (matchingUser != null)
            {
                matchingUser.Active = true;
                db.SubmitChanges();
                return true;
            }
            return false;
        }
    }
}
