//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Linq;

    /// <summary>
    /// Extension of the partial LINQ class Message
    /// </summary>
    public partial class Message
    {
        /// <summary>
        /// Marks this message as received/read.
        /// </summary>
        public void MarkAsReceived()
        {
            CosmoMongerDbDataContext db = CosmoManager.GetDbContext();
            this.Received = true;

            // Save database changes
            db.SaveChanges();
        }
    }
}
