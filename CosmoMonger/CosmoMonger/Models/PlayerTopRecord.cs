//-----------------------------------------------------------------------
// <copyright file="PlayerTopRecord.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// This class repersents an entry in the top player records
    /// </summary>
    public class PlayerTopRecord
    {
        /// <summary>
        /// The player this record is for
        /// </summary>
        private Player player;

        /// <summary>
        /// The type of the record, NetWorth, CargoLooted, etc
        /// </summary>
        private Player.RecordType recordType;

        /// <summary>
        /// The actual value of the record, could be integer or float
        /// </summary>
        private object recordValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerTopRecord"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="recordType">Type of the record.</param>
        /// <param name="recordValue">The record value.</param>
        public PlayerTopRecord(Player player, Player.RecordType recordType, object recordValue)
        {
            this.player = player;
            this.recordType = recordType;
            this.recordValue = recordValue;
        }

        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>The player.</value>
        public Player Player
        {
            get { return this.player; }
            set { this.player = value; }
        }

        /// <summary>
        /// Gets or sets the type of the record.
        /// </summary>
        /// <value>The type of the record.</value>
        public Player.RecordType RecordType
        {
            get { return this.recordType; }
            set { this.recordType = value; }
        }

        /// <summary>
        /// Gets or sets the record value.
        /// </summary>
        /// <value>The record value.</value>
        public object RecordValue
        {
            get { return this.recordValue; }
            set { this.recordValue = value; }
        }

        /// <summary>
        /// Gets the record value as a formatted string.
        /// </summary>
        /// <value>The  record value as formatted string.</value>
        /// <returns>The record value formatted as a string</returns>
        public string FormattedRecordValue
        {
            get
            {
                switch (this.recordType)
                {
                    case Player.RecordType.NetWorth:
                        return string.Format("{0:C0}", this.recordValue);

                    case Player.RecordType.BountyCollected:
                        return string.Format("{0:C0}", this.recordValue);

                    case Player.RecordType.Bounty:
                        return string.Format("{0:C0}", this.recordValue);

                    case Player.RecordType.ShipsDestroyed:
                        return string.Format("{0}", this.recordValue);

                    case Player.RecordType.ForcedSurrenders:
                        return string.Format("{0}", this.recordValue);

                    case Player.RecordType.ForcedFlees:
                        return string.Format("{0}", this.recordValue);

                    case Player.RecordType.CargoLootedWorth:
                        return string.Format("{0:C0}", this.recordValue);

                    case Player.RecordType.ShipsLost:
                        return string.Format("{0}", this.recordValue);

                    case Player.RecordType.SurrenderCount:
                        return string.Format("{0}", this.recordValue);

                    case Player.RecordType.FleeCount:
                        return string.Format("{0}", this.recordValue);

                    case Player.RecordType.CargoLostWorth:
                        return string.Format("{0:C0}", this.recordValue);

                    case Player.RecordType.DistanceTraveled:
                        return string.Format("{0:N02}", this.recordValue);

                    case Player.RecordType.GoodsTraded:
                        return string.Format("{0}", this.recordValue);

                    default:
                        throw new ArgumentException("Unhandled recordType in GetRecordAsString", "recordType");
                }
            }
        }

        /// <summary>
        /// Converts the player record type to string.
        /// </summary>
        /// <param name="recordType">Type of the player record to convert.</param>
        /// <returns>A nice string name</returns>
        public static string ConvertRecordTypeToString(Player.RecordType recordType)
        {
            switch (recordType)
            {
                case Player.RecordType.Bounty:
                    return "Player Bounty";
                case Player.RecordType.ShipsDestroyed:
                    return "Opponent's Destroyed";
                case Player.RecordType.ForcedSurrenders:
                    return "Surrendered Opponents";
                case Player.RecordType.ForcedFlees:
                    return "Fled Opponents";
                case Player.RecordType.CargoLootedWorth:
                    return "Captured Cargo";
                case Player.RecordType.SurrenderCount:
                    return "Times Surrendered";
                case Player.RecordType.FleeCount:
                    return "Times Fled";
                case Player.RecordType.CargoLostWorth:
                    return "Lost Cargo";
                default:
                    return Regex.Replace(recordType.ToString(), "([A-Z])", " $1", RegexOptions.Compiled).Trim();
            }
        }
    }
}
