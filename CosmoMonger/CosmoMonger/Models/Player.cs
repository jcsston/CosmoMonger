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
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Linq;
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
                netWorth += this.Ship.TradeInValue + this.Ship.ShipGoods.Sum(x => x.Quantity * x.Good.BasePrice);
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

                // Send changes to database
                CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
                db.SubmitChanges();
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
    }
}
