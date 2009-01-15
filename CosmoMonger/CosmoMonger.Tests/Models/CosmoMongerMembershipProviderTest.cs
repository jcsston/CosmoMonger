namespace CosmoMonger.Tests.Models
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using CosmoMonger.Models;
    using NUnit.Framework;
    
    /// <summary>
    /// Summary description for CosmoMongerMembershipProviderTest
    /// </summary>
    [TestFixture]
    public class CosmoMongerMembershipProviderTest
    {
        [Test]
        public void CosmoMongerMembershipProviderMaxInvalidPasswordAttempts()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider();
            int actual = target.MaxInvalidPasswordAttempts;
            Assert.AreEqual(3, actual, "Verify the correctness of this test method.");
        }
    }
}
