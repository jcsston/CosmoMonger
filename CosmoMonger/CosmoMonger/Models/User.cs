namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extension of the partial LINQ class User
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// This creates a player in the database and returns a reference to the new player. 
        /// If the player already exists an ArgumentException will be thrown, referencing the name argument.
        /// </summary>
        /// <param name="name">The player name.</param>
        /// <param name="race">The race of the new Player.</param>
        /// <returns>The newly created Player</returns>
        public Player CreatePlayer(string name, Race race)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            bool existingPlayerName = (from p in db.Players where p.Name == name select p).Any();
            if (existingPlayerName)
            {
                throw new ArgumentException("Player with same name already exists", "name");
            }

            Player player = new Player();
            player.User = this;
            player.Name = name;
            player.Race = race;
            db.Players.InsertOnSubmit(player);
            db.SubmitChanges();

            return player;
        }

        /// <summary>
        /// Update the user profile with the provided arguments. 
        /// Raises an ArgumentException if the email or username is invalid or already taken.
        /// </summary>
        /// <param name="username">The username to set.</param>
        /// <param name="email">The email to set.</param>
        public void UpdateProfile(string username, string email)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns an list of BuddyList objects for this User
        /// </summary>
        /// <returns>Array of BuddyList objects</returns>
        public BuddyList[] GetBuddyList()
        {
            return this.BuddyLists.ToArray();
        }

        /// <summary>
        /// Adds the passed in user to the users buddy list. 
        /// If the user is already in the list an ArgumentException is thrown.
        /// </summary>
        /// <param name="buddy">The buddy.</param>
        public void AddBuddy(User buddy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the passed in user from the users buddy list. 
        /// If the user is not in the buddy list, an ArgumentException is thrown.
        /// </summary>
        /// <param name="buddy">The buddy.</param>
        public void RemoveBuddy(User buddy)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an list of IgnoreList objects for this User
        /// </summary>
        /// <returns>Array of IgnoreList objects</returns>
        public IgnoreList[] GetIgnoreList()
        {
            return this.IgnoreLists.ToArray();
        }

        /// <summary>
        /// Adds the passed in user to the users ignore list. If the user is already in the list an ArgumentException is thrown.
        /// </summary>
        /// <param name="ignoreUser">The ignore user.</param>
        public void AddIgnore(User ignoreUser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the passed in user from the users ignore list. If the user is not in the ignore list, an ArgumentException is thrown.
        /// </summary>
        /// <param name="ignoreUser">The ignore user.</param>
        public void RemoveIgnore(User ignoreUser)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This returns any unread messages for the User. 
        /// If no unread messages exist an empty array is returned.
        /// </summary>
        /// <returns>Array of Message objects</returns>
        public Message[] FetchUnreadMessages()
        {
            return (from m in Messages where !m.Read select m).ToArray();
        }

        /// <summary>
        /// Send a message to be added to this Users message queue
        /// </summary>
        /// <param name="fromUser">From user.</param>
        /// <param name="message">The message to send.</param>
        public void SendMessage(User fromUser, string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Bans the user by setting the Active field to false.
        /// </summary>
        public void Ban()
        {
            this.Active = false;
        }

    }
}