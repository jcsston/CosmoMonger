//-----------------------------------------------------------------------
// <copyright file="User.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Linq;
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
            bool otherPlayerName = (from p in db.Players 
                                       where p.Name == name 
                                       && p.User != this
                                       select p).Any();
            if (otherPlayerName)
            {
                throw new ArgumentException("Player by another user with the same name already exists", "name");
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "Name", name },
                { "Race", race.Name }
            };
            Logger.Write("Creating player in User.CreatePlayer", "Model", 700, 0, TraceEventType.Information, "Creating Player", props);

            Player player = new Player();
            player.User = this;
            player.Name = name;
            player.Race = race;
            player.Alive = true;
            player.LastPlayed = DateTime.Now;

            // Starting credits is 2000
            player.CashCredits = 2000;

            // Assign the default starting location based on the race
            CosmoSystem startingSystem = race.HomeSystem;
            if (startingSystem == null)
            {
                Logger.Write("Unable to load player starting system from database", "Model", 1000, 0, TraceEventType.Critical);
                return null;
            }

            // Create a new ship for this player
            player.CreateStartingShip(startingSystem);

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
        public virtual void UpdateEmail(string email)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Check that the e-mail doesn't already exist on other user
            bool matchingEmail = (from u in db.Users where u.Email == email && u != this select u).Any();
            if (matchingEmail)
            {
                throw new ArgumentException("Another user has the same e-mail", "email");
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "UserId", this.UserId },
                { "OldEmail", this.Email },
                { "NewEmail", email }
            };
            Logger.Write("Changing user email in User.UpdateEmail", "Model", 500, 0, TraceEventType.Information, "Changing User Email", props);

            this.Email = email;

            // Save changes
            db.SubmitChanges();
        }
        
        /// <summary>
        /// Returns an list of BuddyList objects for this User
        /// </summary>
        /// <returns>Array of BuddyList objects</returns>
        public virtual BuddyList[] GetBuddyList()
        {
            return (from b in BuddyLists
                    orderby b.Friend.UserName
                    select b).ToArray();
        }

        /// <summary>
        /// Adds the passed in user to the users buddy list. 
        /// If the user is already in the list an ArgumentException is thrown.
        /// </summary>
        /// <param name="buddy">The buddy to add.</param>
        /// <exception cref="ArgumentException">Thrown when buddy is already in the buddy list</exception>
        public virtual void AddBuddy(User buddy)
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
        public virtual void RemoveBuddy(User buddy)
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
        public virtual IgnoreList[] GetIgnoreList()
        {
            return this.IgnoreLists.ToArray();
        }

        /// <summary>
        /// Adds the passed in user to the users ignore list. If the user is already in the list an ArgumentException is thrown.
        /// </summary>
        /// <param name="ignoreUser">The user to add to the ignore list.</param>
        /// <exception cref="ArgumentException">Thrown when the ignore user is already in the ignore list</exception>
        public virtual void AddIgnore(User ignoreUser)
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
        public virtual void RemoveIgnore(User ignoreUser)
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
        /// This returns any unread messages for the User.
        /// If no unread messages exist an empty array is returned.
        /// </summary>
        /// <remarks>This function always fetches fresh values from database and is not cached.</remarks>
        /// <returns>Array of Message objects</returns>
        public virtual IEnumerable<Message> GetUnreadMessages()
        {
            // Ensure that the messages are freshly loaded from the database by querying database directly
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from m in db.Messages
                    where m.RecipientUserId == this.UserId
                    && !m.Received
                    && m.VisibleToRecipient
                    select m).AsEnumerable();
        }

        /// <summary>
        /// Send a message to the toUser message queue
        /// </summary>
        /// <param name="toUser">The user to send the message to.</param>
        /// <param name="subject">The message subject.</param>
        /// <param name="message">The message to send.</param>
        public virtual void SendMessage(User toUser, string subject, string message)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Check if this user is on the to user's ignore list
            bool presentOnIgnoreList = (from ilo in this.IgnoreListsOn
                                        where ilo.User == toUser
                                        select ilo).Any();
            if (presentOnIgnoreList)
            {
                // Don't send the message
                return;
            }

            // Build the message
            Message msg = new Message();
            msg.RecipientUser = toUser;
            msg.SenderUser = this;
            msg.Subject = subject;
            msg.Content = message;
            msg.Time = DateTime.Now;
            msg.VisibleToRecipient = true;
            msg.VisibleToSender = true;

            // Add the message to the database
            db.Messages.InsertOnSubmit(msg);

            // Save changes to database
            db.SubmitChanges();
        }

        /// <summary>
        /// Bans the user by setting the Active field to false.
        /// </summary>
        public virtual void Ban()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.Active = false;

            // Save changes to database
            db.SubmitChanges();
        }

        /// <summary>
        /// Unbans the user by setting the Active field to true.
        /// </summary>
        public virtual void Unban()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.Active = true;

            // Save changes to database
            db.SubmitChanges();
        }

        /// <summary>
        /// Gets a message recieved or sent by this user.
        /// </summary>
        /// <param name="messageId">The message id.</param>
        /// <returns>A Message object if found. Null if no message was found.</returns>
        public virtual Message GetMessage(int messageId)
        {
            Message message = (from m in this.Messages
                               where m.MessageId == messageId
                               && m.VisibleToRecipient
                               select m).SingleOrDefault();
            
            // If the message was not found, look in sent messages
            if (message == null)
            {
                message = (from m in this.MessagesSent
                           where m.MessageId == messageId
                           && m.VisibleToSender
                           select m).SingleOrDefault();
            }

            return message;
        }

        /// <summary>
        /// Deletes this message from the database.
        /// </summary>
        /// <param name="messageId">The message id to delete.</param>
        /// <exception cref="ArgumentException">Thrown if the message id is not found.</exception>
        public virtual void DeleteMessage(int messageId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            Message message = (from m in this.Messages
                               where m.MessageId == messageId
                               select m).SingleOrDefault();
            if (message != null)
            {
                message.VisibleToRecipient = false;
            }
            else
            {
                // If the message was not found, look in sent messages
                message = (from m in this.MessagesSent
                           where m.MessageId == messageId
                           select m).SingleOrDefault();
                if (message == null)
                {
                    throw new ArgumentException("Invalid Message Id", "messageId");
                }

                message.VisibleToSender = false;
            }

            db.SubmitChanges();
        }

        /// <summary>
        /// Gets the visible messages for this user.
        /// </summary>
        /// <returns>IEnumerable of Messages</returns>
        public virtual IEnumerable<Message> GetMessages()
        {
            return (from m in this.Messages
                    where m.VisibleToRecipient
                    orderby m.Time descending
                    select m).AsEnumerable();
        }

        /// <summary>
        /// Gets the visible messages sent by this user.
        /// </summary>
        /// <returns>IEnumerable of Messages</returns>
        public virtual IEnumerable<Message> GetMessagesSent()
        {
            return (from m in this.MessagesSent
                    where m.VisibleToSender
                    orderby m.Time descending
                    select m).AsEnumerable();
        }
    }
}