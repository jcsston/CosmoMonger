//-----------------------------------------------------------------------
// <copyright file="NpcBalancer.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// This class balances the number of active NPCs vs active Players to keep the galaxy alive.
    /// </summary>
    public class NpcBalancer : NpcBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBalancer"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcBalancer(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Balances the number of NPCs vs Active Players to keep the galaxy alive.
        /// </summary>
        public override void DoAction()
        {
            // Set next run time to 1 hour
            if (!this.SetNextActionDelay(new TimeSpan(1, 0, 0))) 
            {
                return;
            }

            // Balance Npcs
        }
    }
}
