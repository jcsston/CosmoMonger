//-----------------------------------------------------------------------
// <copyright file="NpcPirate.cs" company="CosmoMonger">
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
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Contains logic for NPC Pirates
    /// </summary>
    public class NpcPirate : NpcBase
    {
        public NpcPirate(Npc npcRow)
            : base(npcRow)
        {

        }
    }
}
