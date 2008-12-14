namespace CosmoMonger.Models
{
    using System;
    using System.Data;
    using System.Configuration;
    using System.Linq;

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
