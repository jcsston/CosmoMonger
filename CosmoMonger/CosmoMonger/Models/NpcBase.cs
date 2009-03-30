//-----------------------------------------------------------------------
// <copyright file="NpcBase.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Base model class for NPC logic
    /// </summary>
    public class NpcBase
    {
        /// <summary>
        /// Holds the NPC row reference
        /// </summary>
        private Npc npcRow = null;

        /// <summary>
        /// Gets the NPC row reference.
        /// </summary>
        /// <value>The NPC row reference.</value>
        protected Npc NpcRow
        {
            get
            {
                return this.npcRow;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBase"/> class.
        /// Base construtor, sets up the npcRow
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcBase(Npc npcRow)
        {
            this.npcRow = npcRow;
        }
    }
}
