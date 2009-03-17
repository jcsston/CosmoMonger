//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
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

            Logger.Write("Withdrawing credits from bank in Player.BankWithdraw", "Model", 500, 0, TraceEventType.Verbose, "Withdrawing credits",
                new Dictionary<string, object>
                {
                    { "PlayerId", this.PlayerId },
                    { "Credits", credits },
                    { "BankCredits", this.BankCredits },
                    { "CashCredits", this.CashCredits }
                }
            );

            this.BankCredits -= credits;
            this.CashCredits += credits;

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            db.SubmitChanges();
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
            if (this.CashCredits < credits)
            {
                throw new ArgumentOutOfRangeException("credits", "Cannot deposit more credits than available in cash");
            }

            Logger.Write("Depositing credits into bank in Player.BankDeposit", "Model", 500, 0, TraceEventType.Verbose, "Depositing credits",
                new Dictionary<string, object>
                {
                    { "PlayerId", this.PlayerId },
                    { "Credits", credits },
                    { "BankCredits", this.BankCredits },
                    { "CashCredits", this.CashCredits }
                }
            );

            this.CashCredits -= credits;
            this.BankCredits += credits;

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            db.SubmitChanges();
        }

        /// <summary>
        /// Updates the net worth for this player.
        /// </summary>
        public virtual void UpdateNetWorth()
        {
            int netWorth = this.BankCredits + this.CashCredits;
            if (this.Ship != null)
            {
                netWorth += this.Ship.TradeInValue + this.Ship.CargoWorth;
            }

            Logger.Write("Updating player net worth in Player.UpdateNetWorth", "Model", 500, 0, TraceEventType.Verbose, "Update player networth",
                new Dictionary<string, object>
                {
                    { "PlayerId", this.PlayerId },
                    { "NewNetWorth", netWorth },
                    { "OldNetWorth", this.NetWorth }
                }
            );

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
            db.SubmitChanges();
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
                TimeSpan playTimeLength = DateTime.Now - this.LastPlayed;

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
                this.LastPlayed = DateTime.Now;

                try
                {
                    // Send changes to database
                    db.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException ex)
                {
                    ExceptionPolicy.HandleException(ex, "SQL Policy");

                    // Another thread has made changes to this Player row, 
                    // Most likely from browsing multiple pages at once.
                    // We will force this update of playtime as this one should be more recent
                    foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                    {
                        // Log each conflict
                        foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                        {
                            string memberDetails = string.Format("{0}.{1} O: {2} D: {3} C: {4}", mcc.Member.DeclaringType.Name, mcc.Member.Name, mcc.OriginalValue, mcc.DatabaseValue, mcc.CurrentValue);
                            Logger.Write(memberDetails, "Model", 10, 0, TraceEventType.Verbose, "SQL Change Conflict Details");
                        }
                        occ.Resolve(RefreshMode.KeepChanges);
                    }
                }
            }
        }

        /// <summary>
        /// A property changed event, called when CashCredits is changed.
        /// </summary>
        partial void OnCashCreditsChanged()
        {
            // Because CashCredits has changed we need to update NetWorth
            this.UpdateNetWorth();
        }

        /// <summary>
        /// A property changed event, called when BankCredits is changed.
        /// </summary>
        partial void OnBankCreditsChanged()
        {
            // Because BankCredits has changed we need to update NetWorth
            this.UpdateNetWorth();
        }

        /// <summary>
        /// Kills this player.
        /// </summary>
        public virtual void Kill()
        {
            Logger.Write("Killing player in Player.Kill", "Model", 600, 0, TraceEventType.Verbose, "Kill Player",
                new Dictionary<string, object>
                {
                    { "PlayerId", this.PlayerId },
                    { "Alive", this.Alive }
                }
            );

            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            
            // Kill this player
            this.Alive = false;

            // Send changes to the database
            db.SubmitChanges();
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

            // Create a new ship for the player
            Ship playerShip = new Ship();

            // Assign the default base ship type
            BaseShip baseShip = (from bs in db.BaseShips
                                 where bs.Name == "Glorified Trash Can"
                                 select bs).SingleOrDefault();
            if (baseShip == null)
            {
                Logger.Write("Unable to load player starting base ship from database", "Model", 1000, 0, TraceEventType.Critical);
                throw new NotSupportedException("Unable to load base ship model from database");
            }

            playerShip.BaseShip = baseShip;
            playerShip.CosmoSystem = startingSystem;

            // Setup default upgrades
            playerShip.JumpDrive = playerShip.BaseShip.InitialJumpDrive;
            playerShip.Shield = playerShip.BaseShip.InitialShield;
            playerShip.Weapon = playerShip.BaseShip.InitialWeapon;

            db.Ships.InsertOnSubmit(playerShip);
            this.Ship = playerShip;
        }
    }
}
