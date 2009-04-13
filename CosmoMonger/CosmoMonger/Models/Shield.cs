//-----------------------------------------------------------------------
// <copyright file="Shield.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extenstion of LINQ class Shield.
    /// </summary>
    public partial class Shield
    {
        /// <summary>
        /// Gets the trade in value for this Shield upgrade
        /// </summary>
        /// <param name="currentShip">The current ship to base the trade in value on.</param>
        /// <returns>The trade in value.</returns>
        public virtual int GetTradeInValue(Ship currentShip)
        {
            return (int)(this.BasePrice * currentShip.BaseShip.Level * 0.80);
        }
    }
}
