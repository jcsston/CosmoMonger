//-----------------------------------------------------------------------
// <copyright file="NpcPolice.cs" company="CosmoMonger">
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
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Contains logic for NPC Police
    /// </summary>
    public class NpcPolice : NpcBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcPolice"/> class.
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcPolice(Npc npcRow)
            : base(npcRow)
        {
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        public override void DoAction()
        {
            if (!this.SetNextActionDelay(new TimeSpan(0, 1, 0)))
            {
                return;
            }

            // throw new NotImplementedException();
        }
    }
}
