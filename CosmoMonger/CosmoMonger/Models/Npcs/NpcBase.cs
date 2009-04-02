//-----------------------------------------------------------------------
// <copyright file="NpcBase.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------

namespace CosmoMonger.Models.Npcs
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using System.Web;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// Base model class for NPC logic
    /// </summary>
    public abstract class NpcBase
    {
        /// <summary>
        /// Holds the NPC row reference
        /// </summary>
        private Npc npcRow = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="NpcBase"/> class.
        /// Base construtor, sets up the npcRow
        /// </summary>
        /// <param name="npcRow">The NPC row reference.</param>
        public NpcBase(Npc npcRow)
        {
            this.npcRow = npcRow;
        }

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
        /// Setup the new Npc in the database, not required for all types.
        /// If the Npc has a ship this should be overrided to handle creation of the ship.
        /// </summary>
        public virtual void Setup()
        {
            // Setup some sane defaults for the required fields
            this.NpcRow.Aggression = 0;
            this.NpcRow.NextActionTime = DateTime.UtcNow;

            // Give a default name
            if (this.NpcRow.Name == null)
            {
                this.NpcRow.Name = this.NpcRow.NType.ToString();
            }
        }

        /// <summary>
        /// Does the action for the Npc. Needs to call SetNextActionDelay to schedule next call time.
        /// </summary>
        public abstract void DoAction();

        /// <summary>
        /// Sets the delay before the next action will be taken for this Npc again.
        /// </summary>
        /// <param name="delay">The delay before the next scheduled action.</param>
        /// <returns>true is time has been set, false if another thread has already ran (Npc should exit)</returns>
        protected bool SetNextActionDelay(TimeSpan delay)
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();

            // Mark the next time this NPC will need to do an action
            this.npcRow.NextActionTime = DateTime.UtcNow.Add(delay);

            try
            {
                // Send changes to database
                db.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes to this Npc object, 
                // which means another thread has already started
                // We shouldn't redo that work, and so we exit
                foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                {
                    // Refresh current values from database
                    occ.Resolve(RefreshMode.OverwriteCurrentValues);
                }

                return false;
            }

            return true;
        }
    }
}
