﻿//-----------------------------------------------------------------------
// <copyright file="SystemGood.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using System.Data.SqlClient;
    using System.Configuration;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Extension of the partial LINQ class SystemGood
    /// </summary>
    public partial class SystemGood
    {
        /// <summary>
        /// Gets the actual price of the good.
        /// Calcuated by taking the Good.BasePrice and the SystemGood.PriceMultiplier
        /// </summary>
        public int Price
        {
            get
            {
                return (int)(this.Good.BasePrice * this.PriceMultiplier);
            }
        }

        /// <summary>
        /// Buys the specified quantity of goods from the system.
        /// </summary>
        /// <param name="manager">The current GameManager object.</param>
        /// <param name="quantity">The quantity of the good to buy.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when trying to buy more goods than avaiable in the system.</exception>
        /// <exception cref="ArgumentException">Thrown when there is not enough credits or cargo space to buy the requested number of goods.</exception>
        public virtual void Buy(GameManager manager, int quantity)
        {
            // Check that we are not trying to buy more goods than there is
            if (this.Quantity < quantity)
            {
                throw new ArgumentOutOfRangeException("quantity", quantity, "Unable to buy more goods than at the system");
            }

            // Check if the player has enough money to buy the goods
            int totalCost = (int)this.Price * quantity;
            if (manager.CurrentPlayer.CashCredits < totalCost)
            {
                throw new ArgumentException("Not enough credits to buy requested number of goods", "quantity");
            }
            
            // Check if the player has enough cargo space to carry the goods
            Ship playerShip = manager.CurrentPlayer.Ship;
            if (playerShip.CargoSpaceFree < quantity)
            {
                throw new ArgumentException("Not enough cargo space to carry requested number of goods", "quantity");
            }

            Logger.Write("Buying goods in SystemGood.Buy", "Model", 500, 0, TraceEventType.Information, "Buying Goods",
                new Dictionary<string, object>
                {
                    { "PlayerId", manager.CurrentPlayer.PlayerId },
                    { "SystemId", this.SystemId },
                    { "GoodId", this.GoodId },
                    { "Quantity", quantity },
                    { "TotalCost", totalCost }
                }
            );

            // Remove the goods from the system
            this.Quantity -= quantity;

            // We don't delete the SystemGood even if quantity is 0 because the SystemGood also signals if the system buys the good

            // Add the goods to the player ship
            ShipGood playerGood = (from sg in playerShip.ShipGoods
                                   where sg.Good == this.Good
                                   select sg).SingleOrDefault();
            if (playerGood == null)
            {
                // Ship is not already carrying this good, so we have to create a new ShipGood
                playerGood = new ShipGood();
                playerGood.Ship = playerShip;
                playerGood.Good = this.Good;
                playerGood.Quantity = quantity;
                playerShip.ShipGoods.Add(playerGood);
            }
            else
            {
                // Ship is already carrying this good, add to the existing ShipGood
                playerGood.Quantity += quantity;
            }

            // Charge the player for the goods
            manager.CurrentPlayer.CashCredits -= totalCost;

            // Commit changes to the database
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            db.SubmitChanges();
        }

        public virtual Dictionary<DateTime, int> GetPriceHistory()
        {
            Dictionary<DateTime, int> priceHistory = new Dictionary<DateTime, int>();
            Regex priceMultiplierRegex = new Regex("PriceMultiplier: (\\d+.\\d+)");
            SqlConnection logConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["LoggingConnectionString"].ConnectionString);
            logConnection.Open();
            using (logConnection)
            {
                SqlCommand priceHistoryCmd = logConnection.CreateCommand();
                priceHistoryCmd.CommandText = "SELECT Timestamp, FormattedMessage FROM Log WHERE Title = 'Adjusting Good Price' AND FormattedMessage LIKE '%SystemId: " + this.SystemId + "\r%' AND FormattedMessage LIKE '%GoodId: " + this.GoodId + "\r%' ORDER BY Timestamp";

                SqlDataReader reader = priceHistoryCmd.ExecuteReader();
                while (reader.Read())
                {
                    DateTime timestamp = (DateTime)reader["Timestamp"];
                    string formattedMessage = reader["FormattedMessage"] as string;
                    if (formattedMessage != null)
                    {
                        Match match = priceMultiplierRegex.Match(formattedMessage);
                        if (match != null)
                        {
                            double priceMultipler = double.Parse(match.Groups[1].Value);
                            int price = (int)(priceMultipler * this.Good.BasePrice);
                            priceHistory[timestamp] = price;
                        }
                    }
                }
                reader.Close();
            }

            return priceHistory;
        }
    }
}
