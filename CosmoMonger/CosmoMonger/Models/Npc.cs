//-----------------------------------------------------------------------
// <copyright file="Npc.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
// <author>Roger Boykin</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using CosmoMonger.Models.Npcs;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Extends the partial LINQ NPC class.
    /// </summary>
    public partial class Npc
    {
        /// <summary>
        /// Does the action.
        /// </summary>
        public virtual void DoAction()
        {
            Logger.Write("Enter Npc.DoAction", "NPC", 200, 0, TraceEventType.Verbose);
            NpcBase npc = null;
            switch (this.NType)
            {
                case NpcType.GoodBalancer:
                    // Special system good price/count balancer NPC
                    npc = new NpcGoodBalancer(this);
                    break;
                case NpcType.NpcBalancer:
                    // Npc Balancer NPC
                    npc = new NpcBalancer(this);
                    break;
                case NpcType.Cleaner:
                    // Cleaner NPC
                    npc = new NpcCleaner(this);
                    break;
                case NpcType.Trader:
                    // Trader NPC
                    npc = new NpcTrader(this);
                    break;
                case NpcType.Pirate:
                    // Pirate NPC
                    npc = new NpcPirate(this);
                    break;
                case NpcType.Police:
                    // Police NPC
                    npc = new NpcPolice(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("NType", this.NType, "Invalid NPC Type");
            }

            // Do the actual NPC action
            npc.DoAction();
        }
    }
}
