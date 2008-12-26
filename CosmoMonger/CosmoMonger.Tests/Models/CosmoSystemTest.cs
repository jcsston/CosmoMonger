namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CosmoMonger.Models;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class CosmoSystemTest
    {
        [Test]
        public void AddGood()
        {
            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            CosmoSystem firstSystem = (from s in db.CosmoSystems select s).FirstOrDefault();
            Assert.That(firstSystem, Is.Not.Null, "We need at least one system in the galaxy");

            // Add some water to the the system
            Good water = (from g in db.Goods
                          where g.Name == "Water"
                          select g).SingleOrDefault();
            Assert.That(water, Is.Not.Null, "We should have a Water good");
            firstSystem.AddGood(water.GoodId, 100);

            // Verify that the good was added to the system
            SystemGood systemWater = firstSystem.GetGood(water.GoodId);
            Assert.That(systemWater, Is.Not.Null, "System should now have a water SystemGood");
            Assert.That(systemWater.Quantity, Is.GreaterThanOrEqualTo(100), "System should now have at least 100 water goods");
        }
    }
}
