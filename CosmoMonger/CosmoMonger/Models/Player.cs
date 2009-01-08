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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();

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
