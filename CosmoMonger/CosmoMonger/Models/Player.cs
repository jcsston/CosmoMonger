//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extension of the partial LINQ class Player
    /// </summary>
    public partial class Player
    {
        /// <summary>
        /// Name of the starting player ship
        /// </summary>
        public const string StartingShip = "Glorified Trash Can";

        public enum RecordType
        {
            NetWorth, 
            BountyCollected, Bounty, 
            ShipsDestroyed, ForcedSurrenders, ForcedFlees,
            CargoLootedWorth, ShipsLost, SurrenderCount, 
            FleeCount, CargoLostWorth, 
            DistanceTraveled, GoodsTraded
        }

        /// <summary>
        /// Gets the reputation level.
        /// </summary>
        /// <value>The reputation level.</value>
        public string ReputationLevel
        {
            get
            {
                if (this.Reputation > 5)
                {
                    return "Good";
                }
                else if (this.Reputation < -5)
                {
                    return "Evil";
                }
                else
                {
                    return "Neutral";
                }
            }
        }

        /// <summary>
        /// Withdraw credits from the Bank.
        /// </summary>
        /// <param name="credits">The amount of credits to withdraw.</param>
        /// <exception cref="InvalidOperationException">Thrown in the system the player currently is in doesn't have a bank</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if more credits than available are withdrawn</exception>
        public virtual void BankWithdraw(int credits)
        {
            // Check that there is a bank in the current system
            if (!this.Ship.CosmoSystem.HasBank)
            {
                throw new InvalidOperationException("No bank available for withdraw from");
            }

            // Check that the credits is postive
            if (0 >= credits)
            {
                throw new ArgumentOutOfRangeException("credits", "Cannot withdraw a negative number of credits");
            }

            // Check that the player has enough credits to withdraw
            if (this.BankCredits < credits)
            {
                throw new ArgumentOutOfRangeException("credits", "Cannot withdraw more credits than available in the bank");
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "PlayerId", this.PlayerId },
                { "Credits", credits },
                { "BankCredits", this.BankCredits },
                { "ShipCredits", this.Ship.Credits }
            };
            Logger.Write("Withdrawing credits from bank in Player.BankWithdraw", "Model", 500, 0, TraceEventType.Verbose, "Withdrawing credits", props);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.BankCredits -= credits;
            this.Ship.Credits += credits;

            // Save database changes
            db.SaveChanges();
        }

        /// <summary>
        /// Deposit credits in the Bank.
        /// </summary>
        /// <param name="credits">The amount of credits to deposit.</param>
        /// <exception cref="InvalidOperationException">Thrown in the system the player currently is in doesn't have a bank</exception>
        /// /// <exception cref="ArgumentOutOfRangeException">Thrown if more credits than available are deposited</exception>
        public virtual void BankDeposit(int credits)
        {
            // Check that there is a bank in the current system
            if (!this.Ship.CosmoSystem.HasBank)
            {
                throw new InvalidOperationException("No bank available to deposit in");
            }

            // Check that the credits is postive
            if (0 >= credits)
            {
                throw new ArgumentOutOfRangeException("credits", "Cannot deposit a negative number of credits");
            }

            // Check that the player has enough credits to deposit
            if (this.Ship.Credits < credits)
            {
                throw new ArgumentOutOfRangeException("credits", "Cannot deposit more credits than available in cash");
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "PlayerId", this.PlayerId },
                { "Credits", credits },
                { "BankCredits", this.BankCredits },
                { "ShipCredits", this.Ship.Credits }
            };
            Logger.Write("Depositing credits into bank in Player.BankDeposit", "Model", 500, 0, TraceEventType.Verbose, "Depositing credits", props);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            this.Ship.Credits -= credits;
            this.BankCredits += credits;

            // Save database changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the net worth for this player.
        /// </summary>
        public virtual void UpdateNetWorth()
        {
            int netWorth = this.BankCredits + this.Ship.Credits;
            if (this.Ship != null)
            {
                netWorth += this.Ship.TradeInValue + this.Ship.CargoWorth;
            }

            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "PlayerId", this.PlayerId },
                { "NewNetWorth", netWorth },
                { "OldNetWorth", this.NetWorth }
            };
            Logger.Write("Updating player net worth in Player.UpdateNetWorth", "Model", 500, 0, TraceEventType.Verbose, "Update player networth", props);

            this.NetWorth = netWorth;

            // Do not call SubmitChanges here as this method is called during modification of the player data
        }

        /// <summary>
        /// Updates the player profile with the new player name. 
        /// Throws an ArgumentException if an existing player with the same name already exists.
        /// </summary>
        /// <param name="name">The new name of the player.</param>
        public virtual void UpdateProfile(string name)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Check for another living player with same name
            bool matchingName = (from p in db.Players where p.Name == name && p.Alive && p != this select p).Any();
            if (matchingName)
            {
                throw new ArgumentException("Another player has the same name", "name");
            }

            // Update this player
            this.Name = name;

            // Save database changes
            db.SaveChanges();
        }

        /// <summary>
        /// Updates the player record snapshot, creating a new one if needed.
        /// </summary>
        public virtual void UpdateRecordSnapshot()
        {
            int currentSnapshotAge = (int)(this.TimePlayed - this.LastRecordSnapshotAge);
            
            // If the last snap is older than 1min, we need to create a new one
            if (currentSnapshotAge > 60)
            {
                CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

                // Create new PlayerRecord row
                PlayerRecord record = new PlayerRecord();
                record.PlayerId = this.PlayerId;
                record.RecordTime = DateTime.UtcNow;
                record.TimePlayed = this.TimePlayed;

                // Copy record values
                record.Bounty = this.Bounty;
                record.BountyCollected = this.BountyCollected;
                record.CargoLootedWorth = this.CargoLootedWorth;
                record.CargoLostWorth = this.CargoLostWorth;
                record.FleeCount = this.FleeCount;
                record.ForcedFlees = this.ForcedFlees;
                record.ForcedSurrenders = this.ForcedSurrenders;
                record.NetWorth = this.NetWorth;
                record.Reputation = this.Reputation;
                record.ShipsDestroyed = this.ShipsDestroyed;
                record.ShipsLost = this.ShipsLost;
                record.SurrenderCount = this.SurrenderCount;
                record.GoodsTraded = this.GoodsTraded;
                record.DistanceTraveled = this.DistanceTraveled;

                // Insert record
                db.PlayerRecords.InsertOnSubmit(record);

                // Update snapshot age
                this.LastRecordSnapshotAge = (int)this.TimePlayed;

                // Save database changes
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Updates the play time for this player.
        /// </summary>
        public virtual void UpdatePlayTime()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            if (this.Alive)
            {
                // Calcuate time since last play
                TimeSpan playTimeLength = DateTime.UtcNow - this.LastPlayed;

                // Login timeout is 5 minutes, so we ignore times greater than 5 minutes
                if (playTimeLength.TotalMinutes < 5)
                {
                    // Update the time played
                    this.TimePlayed += playTimeLength.TotalSeconds;

                    // Check if the time player is past 7 days
                    if (this.TimePlayed > 60 * 60 * 24 * 7)
                    {
                        // Player has reached the time limit
                        this.Alive = false; // Die, die, die!!!
                    }
                }

                // Update last play datetime
                this.LastPlayed = DateTime.UtcNow;

                // Save database changes
                db.SaveChanges();

                // Update player records
                this.UpdateRecordSnapshot();
            }
        }

        /// <summary>
        /// Kills this player.
        /// </summary>
        public virtual void Kill()
        {
            Dictionary<string, object> props = new Dictionary<string, object>
            {
                { "PlayerId", this.PlayerId },
                { "Alive", this.Alive }
            };
            Logger.Write("Killing player in Player.Kill", "Model", 600, 0, TraceEventType.Verbose, "Kill Player", props);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            
            // Kill this player
            this.Alive = false;

            // Save database changes
            db.SaveChanges();
        }

        /// <summary>
        /// Creates the starting ship.
        /// Note, changes are not submitted to database
        /// </summary>
        /// <param name="startingSystem">The starting system.</param>
        public void CreateStartingShip(CosmoSystem startingSystem)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            if (this.Ship != null)
            {
                throw new InvalidOperationException("Player already has a ship");
            }

            // Create new player ship
            this.Ship = startingSystem.CreateShip(Player.StartingShip);
        }

        /// <summary>
        /// A property changed event, called when BankCredits is changed.
        /// </summary>
        partial void OnBankCreditsChanged()
        {
            // Because BankCredits has changed we need to update NetWorth
            this.UpdateNetWorth();
        }
    }
}
