//-----------------------------------------------------------------------
// <copyright file="PriceTableEntry.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper class for Good Price Table.
    /// Contains the system name and a dict of good names/prices for that system.
    /// </summary>
    public class PriceTableEntry
    {
        /// <summary>
        /// The name of the system this entry is for
        /// </summary>
        private string systemName;

        /// <summary>
        /// The good name/price dictionary
        /// </summary>
        private Dictionary<string, int> goodPrices = new Dictionary<string, int>();

        /// <summary>
        /// Gets or sets the name of the system this entry is for.
        /// </summary>
        /// <value>The name of the system this object is for.</value>
        public string SystemName
        {
            get { return this.systemName; }
            set { this.systemName = value; }
        }

        /// <summary>
        /// Gets or sets the dictionary of good names/prices for the system this entry is for.
        /// </summary>
        /// <value>The good name/price dictionary.</value>
        public Dictionary<string, int> GoodPrices
        {
            get { return this.goodPrices; }
            set { this.goodPrices = value; }
        }
    }
}
