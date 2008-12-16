//-----------------------------------------------------------------------
// <copyright file="User.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
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
            player.Alive = true;
            player.LastPlayed = DateTime.Now;

            // Starting credits is 2000
            player.CashCredits = 2000;

            // Create a new ship for this player
            Ship playerShip = new Ship();

            // Assign the default base ship type
            BaseShip baseShip = (from bs in db.BaseShips
                                 where bs.Model == "Glorified Trash Can"
                                 select bs).SingleOrDefault();
            if (baseShip == null)
            {
                Logger.Write("Unable to load player starting base ship from database", "Model", 9999, 3, TraceEventType.Critical);
                return null;
            }
            playerShip.BaseShip = baseShip;

            // Assign the default starting location
            CosmoSystem startingSystem = (from s in db.CosmoSystems
                                          where s.Name == "Sol"
                                          select s).SingleOrDefault();
            if (startingSystem == null)
            {
                Logger.Write("Unable to load player starting system from database", "Model", 9999, 4, TraceEventType.Critical);
                return null;
            }
            playerShip.CosmoSystem = startingSystem;

            // Setup default upgrades
            playerShip.JumpDrive = playerShip.BaseShip.InitialJumpDrive;
            playerShip.Shield = playerShip.BaseShip.InitialShield;
            playerShip.Weapon = playerShip.BaseShip.InitialWeapon;

            db.Ships.InsertOnSubmit(playerShip);
            player.Ship = playerShip;

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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            // Check that the username/email doesn't already exist on other user
            bool matchingUsername = (from u in db.Users where u.UserName == username && u != this select u).Any();
            if (matchingUsername)
            {
                throw new ArgumentException("Another user has the same username", "username");
            }

            bool matchingEmail = (from u in db.Users where u.Email == email && u != this select u).Any();
            if (matchingEmail)
            {
                throw new ArgumentException("Another user has the same e-mail", "email");
            }

            this.UserName = username;
            this.Email = email;

            // Save changes
            db.SubmitChanges();
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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            bool matchingBuddy = (from bl in this.BuddyLists where bl.FriendId == buddy.UserId select bl).Any();
            if (matchingBuddy)
            {
                throw new ArgumentException("User is already in the buddy list", "buddy");
            }

            BuddyList buddyEntry = new BuddyList();
            buddyEntry.User = this;
            buddyEntry.FriendId = buddy.UserId;
            this.BuddyLists.Add(buddyEntry);
            db.SubmitChanges();
        }

        /// <summary>
        /// Removes the passed in user from the users buddy list. 
        /// If the user is not in the buddy list, an ArgumentException is thrown.
        /// </summary>
        /// <param name="buddy">The buddy.</param>
        public void RemoveBuddy(User buddy)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            BuddyList buddyToRemove = (from bl in this.BuddyLists where bl.FriendId == buddy.UserId select bl).SingleOrDefault();
            if (buddyToRemove == null)
            {
                throw new ArgumentException("User is not in the buddy list", "buddy");
            }

            this.BuddyLists.Remove(buddyToRemove);
            db.SubmitChanges();
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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            bool matchingAntiFriend = (from il in this.IgnoreLists where il.AntiFriendId == ignoreUser.UserId select il).Any();
            if (matchingAntiFriend)
            {
                throw new ArgumentException("User is already in the ignore list", "ignoreUser");
            }

            IgnoreList ignoreEntry = new IgnoreList();
            ignoreEntry.User = this;
            ignoreEntry.AntiFriendId = ignoreUser.UserId;
            this.IgnoreLists.Add(ignoreEntry);
            db.SubmitChanges();
        }

        /// <summary>
        /// Removes the passed in user from the users ignore list. If the user is not in the ignore list, an ArgumentException is thrown.
        /// </summary>
        /// <param name="ignoreUser">The ignore user.</param>
        public void RemoveIgnore(User ignoreUser)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            IgnoreList antiFriendToRemove = (from il in this.IgnoreLists where il.AntiFriendId == ignoreUser.UserId select il).SingleOrDefault();
            if (antiFriendToRemove == null)
            {
                throw new ArgumentException("User is not in the buddy list", "buddy");
            }

            this.IgnoreLists.Remove(antiFriendToRemove);
            db.SubmitChanges();
        }

        /// <summary>
        /// This returns any unread messages for the User. 
        /// If no unread messages exist an empty array is returned.
        /// </summary>
        /// <returns>Array of Message objects</returns>
        public Message[] FetchUnreadMessages()
        {
            return (from m in Messages where !m.Received select m).ToArray();
        }

        /// <summary>
        /// Send a message to be added to this Users message queue
        /// </summary>
        /// <param name="fromUser">From user.</param>
        /// <param name="message">The message to send.</param>
        public void SendMessage(User fromUser, string message)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

            // Build the message
            Message msg = new Message();
            msg.RecipientUser = this;
            msg.SenderUserId = fromUser.UserId;
            msg.Content = message;
            msg.Time = DateTime.Now;

            // Add the message to this user
            this.Messages.Add(msg);

            // Save changes to database
            db.SubmitChanges();
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