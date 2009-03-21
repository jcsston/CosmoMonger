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
        public const int BaseAccuracy = 75;
    }
}
