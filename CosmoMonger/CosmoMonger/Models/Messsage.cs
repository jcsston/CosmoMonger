//-----------------------------------------------------------------------
// <copyright file="Message.cs" company="CosmoMonger">
//     Copyright (c) 2008 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Data;
    using System.Configuration;
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
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            this.Received = true;
            db.SubmitChanges();
        }
    }
}
