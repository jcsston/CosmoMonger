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
        /// This construtor is intended for unit-testing purposes.
        /// </summary>
        /// <param name="currentUser">The User object of the currently logged in user.</param>
        public GameManager(User currentUser)
        {
            this.currentUser = currentUser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameManager"/> class.
        /// </summary>
        /// <param name="username">The username of the currently logged in user.</param>
        public GameManager(string username)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
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
        public virtual Player CurrentPlayer
        {
            get
            {
                return (from p in currentUser.Players where p.Alive select p).SingleOrDefault();
            }
        }

        /// <summary>
        /// Returns the user with the passed in id. Returns null if the user does not exist.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>User object, null if the user does not exist.</returns>
        public virtual User GetUser(int userId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
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
        public virtual Player[] GetTopPlayers(string recordType, int limit)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            switch (recordType)
            {
                case "NetWorth":
                    return (from p in db.Players orderby p.NetWorth descending, p.Name select p).Take(limit).ToArray();

                case "BountyTotal":
                    return (from p in db.Players orderby p.BountyTotal descending, p.Name select p).Take(limit).ToArray();

                case "HighestBounty":
                    return (from p in db.Players orderby p.HighestBounty descending, p.Name select p).Take(limit).ToArray();

                case "ShipsDestroyed":
                    return (from p in db.Players orderby p.ShipsDestroyed descending, p.Name select p).Take(limit).ToArray();

                case "ForcedSurrenders":
                    return (from p in db.Players orderby p.ForcedSurrenders descending, p.Name select p).Take(limit).ToArray();

                case "ForcedFlees":
                    return (from p in db.Players orderby p.ForcedFlees descending, p.Name select p).Take(limit).ToArray();

                case "CargoLooted":
                    return (from p in db.Players orderby p.CargoLootedWorth descending, p.Name select p).Take(limit).ToArray();

                case "ShipsLost":
                    return (from p in db.Players orderby p.ShipsLost descending, p.Name select p).Take(limit).ToArray();

                case "SurrenderCount":
                    return (from p in db.Players orderby p.SurrenderCount descending, p.Name select p).Take(limit).ToArray();

                case "FleeCount":
                    return (from p in db.Players orderby p.FleeCount descending, p.Name select p).Take(limit).ToArray();

                case "CargoLost":
                    return (from p in db.Players orderby p.CargoLostWorth descending, p.Name select p).Take(limit).ToArray();

                default:
                    throw new ArgumentException("Invalid recordType in GetTopPlayers", "recordType");
            }
        }

        /// <summary>
        /// Fetches the Player object for the passed in player id. 
        /// If the player doesn't exist, null is returned.
        /// </summary>
        /// <param name="playerId">The player id.</param>
        /// <returns>Player object, null if the player does not exist.</returns>
        public virtual Player GetPlayer(int playerId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from p in db.Players where p.PlayerId == playerId select p).SingleOrDefault();
        }

        /// <summary>
        /// Return an array of all systems in the galaxy.
        /// </summary>
        /// <returns>Array of CosmoSystem objects</returns>
        public virtual CosmoSystem[] GetSystems()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from s in db.CosmoSystems select s).ToArray();
        }

        /// <summary>
        /// Gets all the races available.
        /// </summary>
        /// <returns>Array of Race objects</returns>
        public virtual Race[] GetRaces()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from r in db.Races select r).ToArray();
        }

        /// <summary>
        /// Gets the race by id number.
        /// </summary>
        /// <param name="raceId">The race id of the Race object to get.</param>
        /// <returns>Race oject, null if the race does not exist</returns>
        public virtual Race GetRace(int raceId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from r in db.Races where r.RaceId == raceId select r).SingleOrDefault();
        }

        /// <summary>
        /// Gets the system by id number.
        /// </summary>
        /// <param name="systemId">The system id of the System object to get.</param>
        /// <returns>CosmoSystem object, null if the system does not exist</returns>
        public virtual CosmoSystem GetSystem(int systemId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from s in db.CosmoSystems where s.SystemId == systemId select s).SingleOrDefault();
        }

        /// <summary>
        /// Gets the size of the galaxy.
        /// </summary>
        /// <returns>An int that gives the x/y size of the galaxy</returns>
        public virtual int GetGalaxySize()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return Math.Max(db.CosmoSystems.Max(x => x.PositionX), db.CosmoSystems.Max(x => x.PositionY));
        }

        /// <summary>
        /// Gets the price table for the whole galaxy
        /// </summary>
        /// <returns>A List of PriceTablEntry objects</returns>
        public virtual List<PriceTableEntry> GetPriceTable()
        {
            List<PriceTableEntry> priceTable = new List<PriceTableEntry>();

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            foreach (CosmoSystem system in db.CosmoSystems)
            {
                PriceTableEntry prices = new PriceTableEntry();
                prices.SystemName = system.Name;
                foreach (Good good in db.Goods)
                {
                    prices.GoodPrices[good.Name] = (from g in system.SystemGoods
                                                    where g.Good == good
                                                    select g.Price).SingleOrDefault();
                }
                priceTable.Add(prices);
            }

            return priceTable;
        }

        public virtual Good [] GetGoods()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from g in db.Goods select g).ToArray();
        }

        /// <summary>
        /// Finds a player by their username or active player name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The IEnumerable of Player objects for the matching user/players.</returns>
        public virtual IEnumerable<Player> FindPlayer(string name)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // We search active player names and their usernames
            return (from p in db.Players
                    where p.Alive
                    && (p.Name.Contains(name) || p.User.UserName.Contains(name))
                    select p);
        }

        /// <summary>
        /// Finds a user by username or past player name
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The IEnumerable of User objects for the matching users.</returns>
        public virtual IEnumerable<User> FindUser(string name)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // We search users and their player names
            return (from u in db.Users
                    where u.UserName.Contains(name)
                    || u.Email.Contains(name)
                    select u)
                    .Union(
                   (from p in db.Players
                    where p.Name.Contains(name)
                    select p.User));
        }

        public virtual Ship GetShip(int shipId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from s in db.Ships
                    where s.ShipId == shipId
                    select s).SingleOrDefault();
        }

        public Combat GetCombat(int combatId)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return (from c in db.Combats
                    where c.CombatId == combatId
                    select c).SingleOrDefault();
        }
    }
}