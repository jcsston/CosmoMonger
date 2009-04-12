//-----------------------------------------------------------------------
// <copyright file="JumpDrive.cs" company="CosmoMonger">
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
    /// Extenstion of LINQ class JumpDrive.
    /// </summary>
    public partial class JumpDrive
    {
        /// <summary>
        /// Gets the trade in value for this Shield upgrade
        /// </summary>
        /// <param name="currentShip">The current ship to base the trade in value on.</param>
        /// <returns></returns>
        /// <value>The trade in value.</value>
        public virtual int GetTradeInValue(Ship currentShip)
        {
            return (int)(this.BasePrice * currentShip.BaseShip.Level * 0.80);
        }
    }
}
