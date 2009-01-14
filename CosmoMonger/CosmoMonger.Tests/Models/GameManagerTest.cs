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
    public class GameManagerTest : BasePlayerTest
    {
        [Test]
        public void ConstructorEmptyUsername()
        {
            GameManager manager = null;
            try
            {
                manager = new GameManager("");
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("username"), "Failed argument should be username");
            }
            Assert.That(manager, Is.Null, "An empty username should fail and not create a GameManager");
        }

        [Test]
        public void GetTopPlayersEmptyRecordType()
        {
            GameManager manager = null;
            try
            {
                manager = new GameManager("");
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("username"), "Failed argument should be username");
            }
            Assert.That(manager, Is.Null, "An empty username should fail and not create a GameManager");
        }
    }
}
