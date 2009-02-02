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
    public class CosmoManagerTest
    {
        [Test]
        public void GetDbContext()
        {
            Assert.That(CosmoManager.GetDbContext(), Is.Not.Null, "We should always be able to get a database context");
        }

        [Test]
        public void GetCodeVersion()
        {
            Assert.That(CosmoManager.GetCodeVersion(), Is.GreaterThan(0), "Code Version should be greater than zero");
        }

        [Test]
        public void GetDatabaseVersion()
        {
            Assert.That(CosmoManager.GetDatabaseVersion(), Is.GreaterThan(0), "Database Version should be greater than zero");
        }

        [Test]
        public void DoPendingNPCActions()
        {
            CosmoManager.DoPendingNPCActions();
        }
    }
}
