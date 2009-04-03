//-----------------------------------------------------------------------
// <copyright file="CosmoManager.cs" company="CosmoMonger">
//     Copyright (c) 2008-2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Linq;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Web;
    using System.Web.Security;
    using CosmoMonger.Models.Utility;
    using Microsoft.Practices.EnterpriseLibrary.Logging;

    /// <summary>
    /// This is the central control class for the CosmoMonger game.
    /// </summary>
    public class CosmoManager
    {
        /// <summary>
        /// Gets the CosmoMonger db context.
        /// </summary>
        /// <returns>LINQ CosmoMongerDbDataContext object</returns>
        public static CosmoMongerDbDataContext GetDbContext()
        {
            CosmoMongerDbDataContext db = DataContextFactory.GetScopedDataContext<CosmoMongerDbDataContext>("CosmoMonger", ConfigurationManager.ConnectionStrings["CosmoMongerConnectionString"].ConnectionString);
            
            // We only add the logger to the DataContext if sql query logging is enabled
            LogEntry sqlEntry = new LogEntry(String.Empty, "SQL", 1, 0, TraceEventType.Verbose, "LINQ SQL", null);
            if (Logger.ShouldLog(sqlEntry))
            {
                db.Log = new Utility.LoggingTextWriter(sqlEntry);
            }
            else
            {
                ////db.Log = Console.Error;
            }

            return db;
        }

        /// <summary>
        /// Gets a new CosmoMonger db context.
        /// Deletes the current one and creates a new one.
        /// Any objects retrived previous must be retrived again with this context to be tracked.
        /// </summary>
        /// <returns>LINQ CosmoMongerDbDataContext object</returns>
        public static CosmoMongerDbDataContext GetDbContextNew()
        {
            DataContextFactory.ClearScopedDataContext<CosmoMongerDbDataContext>();

            return CosmoManager.GetDbContext();
        }

        /// <summary>
        /// Gets the current code version. Ex. Subversion Revision number
        /// </summary>
        /// <returns>Revision number of running code</returns>
        public static int GetCodeVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.Revision;
        }

        /// <summary>
        /// Gets the database version. Ex. The liquibase changelog number
        /// </summary>
        /// <returns>Database version of connected database</returns>
        public static int GetDatabaseVersion()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            return db.ExecuteQuery<int>("SELECT MAX(CAST(Id AS int)) FROM DATABASECHANGELOG WHERE ISNUMERIC(Id) = 1").Single();
        }

        /// <summary>
        /// Calls DoAction on all NPCs in the galaxy.
        /// This method will be called every 5 seconds to keep the NPCs
        /// busy in the galaxy.
        /// </summary>
        /// <param name="ignore">Ignore this parameter, added so that the method sig would match WaitCallback.</param>
        public static void DoPendingNPCActions(object ignore)
        {
            Logger.Write("Enter CosmoMonger.DoPendingNPCActions", "Model", 200, 0, TraceEventType.Verbose);

            CosmoMongerDbDataContext db = CosmoManager.GetDbContextNew();
            

            var npcsNeedingAction = (from n in db.Npcs
                                    where n.NextActionTime < DateTime.UtcNow
                                    select n);
            foreach (Npc npc in npcsNeedingAction)
            {
                npc.DoAction();
            }
        }
    }
}
