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
        /// Gets the verification code sent to the users e-mail address to verify their e-mail.
        /// </summary>
        /// <value>The verification code.</value>
        public string VerificationCode
        {
            get
            {
                string code = this.UserId.ToString();
                return code;
            }
        }

        /// <summary>
        /// This creates a player in the database and returns a reference to the new player. 
        /// If the player already exists an ArgumentException will be thrown, referencing the name argument.
        /// </summary>
        /// <param name="name">The player name.</param>
        /// <param name="race">The race of the new Player.</param>
        /// <returns>The newly created Player</returns>
        public virtual Player CreatePlayer(string name, Race race)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            bool existingPlayerName = (from p in db.Players where p.Name == name select p).Any();
            if (existingPlayerName)
            {
                throw new ArgumentException("Player with same name already exists", "name");
            }

            Logger.Write("Creating player in User.CreatePlayer", "Model", 700, 0, TraceEventType.Information, "Creating Player",
                new Dictionary<string, object>
                {
                    { "Name", name },
                    { "Race", race.Name }
                }
            );

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
                                 where bs.Name == "Glorified Trash Can"
                                 select bs).SingleOrDefault();
            if (baseShip == null)
            {
                Logger.Write("Unable to load player starting base ship from database", "Model", 1000, 0, TraceEventType.Critical);
                return null;
            }

            playerShip.BaseShip = baseShip;

            // Assign the default starting location based on the race
            CosmoSystem startingSystem = race.HomeSystem;
            if (startingSystem == null)
            {
                Logger.Write("Unable to load player starting system from database", "Model", 1000, 0, TraceEventType.Critical);
                return null;
            }

            playerShip.CosmoSystem = startingSystem;

            // Setup default upgrades
            playerShip.JumpDrive = playerShip.BaseShip.InitialJumpDrive;
            playerShip.Shield = playerShip.BaseShip.InitialShield;
            playerShip.Weapon = playerShip.BaseShip.InitialWeapon;

            db.Ships.InsertOnSubmit(playerShip);
            player.Ship = playerShip;

            player.UpdateNetWorth();

            db.Players.InsertOnSubmit(player);
            db.SubmitChanges();

            return player;
        }

        /// <summary>
        /// Update the user's e-mail with the new e-mail address.
        /// </summary>
        /// <param name="email">The e-mail to set.</param>
        /// <exception cref="ArgumentException">Thrown if the e-mail is already taken by another user</exception>
        public void UpdateEmail(string email)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Check that the e-mail doesn't already exist on other user
            bool matchingEmail = (from u in db.Users where u.Email == email && u != this select u).Any();
            if (matchingEmail)
            {
                throw new ArgumentException("Another user has the same e-mail", "email");
            }

            Logger.Write("Changing user email in User.UpdateEmail", "Model", 500, 0, TraceEventType.Information, "Changing User Email",
                new Dictionary<string, object>
                {
                    { "UserId", this.UserId },
                    { "OldEmail", this.Email },
                    { "NewEmail", email }
                }
            );

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
        /// <param name="buddy">The buddy to add.</param>
        /// <exception cref="ArgumentException">Thrown when buddy is already in the buddy list</exception>
        public void AddBuddy(User buddy)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            if (buddy == this)
            {
                throw new ArgumentException("Cannot add self to buddy list", "buddy");
            }

            bool matchingBuddy = (from bl in this.BuddyLists where bl.FriendId == buddy.UserId select bl).Any();
            if (matchingBuddy)
            {
                throw new ArgumentException("User is already in the buddy list", "buddy");
            }

            BuddyList buddyEntry = new BuddyList();
            buddyEntry.User = this;
            buddyEntry.FriendId = buddy.UserId;
            db.BuddyLists.InsertOnSubmit(buddyEntry);

            db.SubmitChanges();
        }

        /// <summary>
        /// Removes the passed in user from the users buddy list. 
        /// If the user is not in the buddy list, an ArgumentException is thrown.
        /// </summary>
        /// <param name="buddy">The buddy to remove.</param>
        /// <exception cref="ArgumentException">Thrown when buddy not in the buddy list</exception>
        public void RemoveBuddy(User buddy)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            BuddyList buddyToRemove = (from bl in this.BuddyLists where bl.FriendId == buddy.UserId select bl).SingleOrDefault();
            if (buddyToRemove == null)
            {
                throw new ArgumentException("User is not in the buddy list", "buddy");
            }

            db.BuddyLists.DeleteOnSubmit(buddyToRemove);
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
        /// <param name="ignoreUser">The user to add to the ignore list.</param>
        /// <exception cref="ArgumentException">Thrown when the ignore user is already in the ignore list</exception>
        public void AddIgnore(User ignoreUser)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            if (ignoreUser == this)
            {
                throw new ArgumentException("Cannot add self to ignore list", "ignoreUser");
            }

            bool matchingAntiFriend = (from il in this.IgnoreLists where il.AntiFriendId == ignoreUser.UserId select il).Any();
            if (matchingAntiFriend)
            {
                throw new ArgumentException("User is already in the ignore list", "ignoreUser");
            }

            IgnoreList ignoreEntry = new IgnoreList();
            ignoreEntry.User = this;
            ignoreEntry.AntiFriendId = ignoreUser.UserId;
            db.IgnoreLists.InsertOnSubmit(ignoreEntry);

            db.SubmitChanges();
        }

        /// <summary>
        /// Removes the passed in user from the users ignore list. If the user is not in the ignore list, an ArgumentException is thrown.
        /// </summary>
        /// <param name="ignoreUser">The ignore user.</param>
        /// <exception cref="ArgumentException">Thrown when the ignore user is not in the ignore list</exception>
        public void RemoveIgnore(User ignoreUser)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            IgnoreList antiFriendToRemove = (from il in this.IgnoreLists where il.AntiFriendId == ignoreUser.UserId select il).SingleOrDefault();
            if (antiFriendToRemove == null)
            {
                throw new ArgumentException("User is not in the ignore list", "ignoreUser");
            }

            db.IgnoreLists.DeleteOnSubmit(antiFriendToRemove);
            db.SubmitChanges();
        }

        /// <summary>
        /// This returns any unread messages for the User. Marking each as read.
        /// If no unread messages exist an empty array is returned.
        /// </summary>
        /// <returns>Array of Message objects</returns>
        public IEnumerable<Message> FetchUnreadMessages()
        {
            IEnumerable<Message> messages = (from m in this.Messages 
                                             where !m.Received 
                                             select m);

            // Mark all these messages are read now
            foreach (Message msg in messages)
            {
                msg.MarkAsReceived();
            }

            return messages;
        }

        /// <summary>
        /// Send a message to the toUser message queue
        /// </summary>
        /// <param name="toUser">The user to send the message to.</param>
        /// <param name="message">The message to send.</param>
        public void SendMessage(User toUser, string message)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Build the message
            Message msg = new Message();
            msg.RecipientUser = toUser;
            msg.SenderUser = this;
            msg.Content = message;
            msg.Time = DateTime.Now;

            // Add the message to the database
            db.Messages.InsertOnSubmit(msg);

            // Save changes to database
            db.SubmitChanges();
        }

        /// <summary>
        /// Bans the user by setting the Active field to false.
        /// </summary>
        public void Ban()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.Active = false;

            // Save changes to database
            db.SubmitChanges();
        }

        /// <summary>
        /// Unbans the user by setting the Active field to true.
        /// </summary>
        public void Unban()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.Active = true;

            // Save changes to database
            db.SubmitChanges();
        }
    }
}