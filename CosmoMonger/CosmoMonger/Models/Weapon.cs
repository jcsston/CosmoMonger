//-----------------------------------------------------------------------
// <copyright file="Weapon.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// Extension of the partial LINQ class Weapon
    /// </summary>
    public partial class Weapon
    {
        /// <summary>
        /// The base accuracy for weapons.
        /// </summary>
        public const double BaseAccuracy = 0.75;

        /// <summary>
        /// Gets the trade in value for this Weapon upgrade
        /// </summary>
        /// <param name="currentShip">The current ship to base the trade in value on.</param>
        /// <returns>The trade in value.</returns>
        public virtual int GetTradeInValue(Ship currentShip)
        {
            return (int)(this.BasePrice * currentShip.BaseShip.Level * 0.80);
        }
    }
}
