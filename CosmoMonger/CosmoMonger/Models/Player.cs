//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Web;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using System.Xml.Linq;

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
        /// Updates the net worth for this player.
        /// </summary>
        public void UpdateNetWorth()
        {
            int netWorth = this.BankCredits + this.CashCredits;
            if (this.Ship != null)
            {
                netWorth += this.Ship.TradeInValue + this.Ship.ShipGoods.Sum(x => x.Quantity * x.Good.BasePrice);
            }

            this.NetWorth = netWorth;
        }

        /// <summary>
        /// Updates the player profile with the new player name. 
        /// Throws an ArgumentException if an existing player with the same name already exists.
        /// </summary>
        /// <param name="name">The new name of the player.</param>
        public void UpdateProfile(string name)
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
        public void UpdatePlayTime()
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
    }
}
