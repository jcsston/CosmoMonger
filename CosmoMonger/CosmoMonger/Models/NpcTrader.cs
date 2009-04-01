﻿//-----------------------------------------------------------------------
// <copyright file="NpcTrader.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Roger Boykin</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Contains logic for NPC Traders
    /// </summary>
    public class NpcTrader : NpcBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcTrader"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcTrader(Npc npcRow)
            : base(npcRow)
        {

        }
    }
}
