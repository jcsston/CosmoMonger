using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CosmoMonger.Models;
using System.Web.Security;

namespace CosmoMonger.Tests.Models
{
    /// <summary>
    /// Summary description for UserTest
    /// </summary>
    [TestClass]
    public class UserTest
    {
        public UserTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            CosmoMongerMembershipProvider provider = new CosmoMongerMembershipProvider();
            MembershipCreateStatus status;
            MembershipUser user = provider.CreateUser("testUser", "test1000", "test@cosmomonger.com", null, null, true, null, out status);
            Assert.AreEqual(MembershipCreateStatus.Success, status, "Test User is created");

            CosmoMongerDbDataContext db = GameManager.GetDbContext();
            User testUser = (from u in db.Users where u.UserName == "testUser" select u).SingleOrDefault();
            Assert.IsNotNull(testUser, "Test User exists in the database");

            testUser.UpdateProfile("testUser1", "test1@cosmomonger.com");

            User testUser1 = (from u in db.Users where u.UserName == "testUser1" select u).SingleOrDefault();
            Assert.IsNotNull(testUser1, "Test User exists in the database");

            provider.DeleteUser("testUser1", false);
        }
    }
}
