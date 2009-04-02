//-----------------------------------------------------------------------
// <copyright file="NpcType.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models.Npcs
{
    using System;

    /// <summary>
    /// Enumeration of the different Npc Types
    /// </summary>
    public enum NpcType
    {
        /// <summary>
        /// Good Balancer NPC Type
        /// </summary>
        GoodBalancer = 1,

        /// <summary>
        /// NPC Balancer NPC Type
        /// </summary>
        NpcBalancer = 2,

        /// <summary>
        /// Cleaner NPC Type
        /// </summary>
        Cleaner = 3,

        /// <summary>
        /// Trader NPC Type
        /// </summary>
        Trader = 4,

        /// <summary>
        /// Pirate NPC Type
        /// </summary>
        Pirate = 5,

        /// <summary>
        /// Police NPC Type
        /// </summary>
        Police = 6
    }
}
