//-----------------------------------------------------------------------
// <copyright file="GameManager.cs" company="CosmoMonger">
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
    using System.Reflection;
    using System.Web;
    using System.Web.Security;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This is the central control class for the CosmoMonger game.
    /// </summary>
    public class GameManager
    {
        /// <summary>
        /// Contains a reference to the current user.
        /// </summary>
        private User currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class.
        /// </summary>
        /// <param name="username">The username of the currently logged in user.</param>
        public GameManager(string username)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            var matchingUsers = (from u in db.Users where u.UserName == username select u);
            if (!matchingUsers.Any())
            {
                throw new ArgumentException("Invalid username in GameManager", "username");
            }

            this.currentUser = matchingUsers.Single();
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                return this.currentUser;
            }
        }

        /// <summary>
        /// Gets the current player
        /// </summary>
        /// <value>The current player.</value>
        public Player CurrentPlayer
        {
            get
            {
                return (from p in currentUser.Players where p.Alive select p).Single();
            }
        }

        /// <summary>
        /// Gets the CosmoMonger db context.
        /// </summary>
        /// <returns>LINQ CosmoMongerDbDataContext object</returns>
        public static CosmoMongerDbDataContext GetDbContext()
        {
            return Utility.DataContextFactory.GetScopedDataContext<CosmoMongerDbDataContext>("CosmoMonger", ConfigurationManager.ConnectionStrings["CosmoMongerConnectionString"].ConnectionString);
        }

        /// <summary>
        /// Gets the current code version. Ex. Subversion Revision number
        /// </summary>
        /// <returns></returns>
        public static int GetCodeVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.Revision;
        }

        /// <summary>
        /// Gets the database version. Ex. The liquibase changelog number
        /// </summary>
        /// <returns></returns>
        public static int GetDatabaseVersion()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            return int.Parse(db.ExecuteQuery<string>("SELECT MAX([ID]) FROM [DATABASECHANGELOG]").Single());
        }

        /// <summary>
        /// Calls DoAction on all NPCs in the galaxy. 
        /// This method will be called every 5 seconds via Cache Expirations to keep the NPCs 
        /// busy in the galaxy even when no human players are hitting pages.
        /// </summary>
        public void DoPendingNPCActions()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the user with the passed in id. Returns null if the user does not exist.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>User object, null if the user does not exist.</returns>
        public User GetUser(int userId)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            return (from u in db.Users where u.UserId == userId select u).SingleOrDefault();
        }

        /// <summary>
        /// Fetches the top records of players based on the recordType argument. 
        /// If the recordType is invalid an ArgumentException is thrown.
        /// </summary>
        /// <param name="recordType">
        /// Type of the record.
        /// Valid recordType values are: 
        /// NetWorth, BountyTotal HighestBounty, ShipsDestroyed, ForcedSurrenders, ForcedFlees, 
        /// CargoLooted, ShipsLost, SurrenderCount, FleeCount, and CargoLost.
        /// </param>
        /// <param name="limit">The number of top players to return</param>
        /// <returns>Array of Player objects</returns>
        public Player[] GetTopPlayers(string recordType, int limit)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            switch (recordType)
            {
                case "NetWorth":
                    return (from p in db.Players orderby p.NetWorth descending select p).Take(limit).ToArray();

                case "BountyTotal":
                    return (from p in db.Players orderby p.BountyTotal descending select p).Take(limit).ToArray();

                case "HighestBounty":
                    return (from p in db.Players orderby p.HighestBounty descending select p).Take(limit).ToArray();

                case "ShipsDestroyed":
                    return (from p in db.Players orderby p.ShipsDestroyed descending select p).Take(limit).ToArray();

                case "ForcedSurrenders":
                    return (from p in db.Players orderby p.ForcedSurrenders descending select p).Take(limit).ToArray();

                case "ForcedFlees":
                    return (from p in db.Players orderby p.ForcedFlees descending select p).Take(limit).ToArray();

                case "CargoLooted":
                    return (from p in db.Players orderby p.CargoLootedWorth descending select p).Take(limit).ToArray();

                case "ShipsLost":
                    return (from p in db.Players orderby p.ShipsLost descending select p).Take(limit).ToArray();

                case "SurrenderCount":
                    return (from p in db.Players orderby p.SurrenderCount descending select p).Take(limit).ToArray();

                case "FleeCount":
                    return (from p in db.Players orderby p.FleeCount descending select p).Take(limit).ToArray();

                case "CargoLost":
                    return (from p in db.Players orderby p.CargoLostWorth descending select p).Take(limit).ToArray();

                default:
                    throw new ArgumentException("Invalid recordType in GetTopPlayers", "recordType");
            }
        }

        /// <summary>
        /// Fetches the Player object for the passed in player id. If the player doesn't exist, null is returned.
        /// </summary>
        /// <param name="playerId">The player id.</param>
        /// <returns>Player object, null if the player does not exist.</returns>
        public Player GetPlayer(int playerId)
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            return (from p in db.Players where p.PlayerId == playerId select p).SingleOrDefault();
        }

        /// <summary>
        /// Return an array of all systems in the galaxy.
        /// </summary>
        /// <returns>Array of CosmoSystem objects</returns>
        public CosmoSystem[] GetSystems()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            return (from s in db.CosmoSystems select s).ToArray();
        }
    }
}