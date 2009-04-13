//-----------------------------------------------------------------------
// <copyright file="CosmoMongerDbDataContext.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
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
    /// Extension of LINQ class CosmoMongerDbDataContext
    /// </summary>
    public partial class CosmoMongerDbDataContext
    {
        /// <summary>
        /// Saves the database changes.
        /// Handles conflicts with RefreshMode.KeepChanges
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                // Send changes to database
                this.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException ex)
            {
                ExceptionPolicy.HandleException(ex, "SQL Policy");

                // Another thread has made changes, we will try to merge in our changes
                foreach (ObjectChangeConflict occ in this.ChangeConflicts)
                {
                    Dictionary<string, object> props = new Dictionary<string, object>();

                    // Add each conflicting member to the dictionary
                    foreach (MemberChangeConflict mcc in occ.MemberConflicts)
                    {
                        string memberName = string.Format("{0}.{1}", mcc.Member.DeclaringType.Name, mcc.Member.Name);
                        string memberValues = string.Format("O: {0} D: {1} C: {2}", mcc.OriginalValue, mcc.DatabaseValue, mcc.CurrentValue);
                        props.Add(memberName, memberValues);
                    }

                    // Log the conflict
                    string logMessage = string.Format("Conflict on {0} object#{1}", occ.Object.GetType().ToString(), occ.GetHashCode());
                    Logger.Write(logMessage, "Model", 10, 0, TraceEventType.Verbose, "SQL Change Conflict", props);

                    // Keep our current changes, but update the other database values
                    occ.Resolve(RefreshMode.KeepChanges);
                }
            }

            // Submit succeeds on second try.
            this.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }
    }
}
