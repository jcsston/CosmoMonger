namespace CosmoMonger.Tests
{
    using CosmoMonger.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
    using System.Web.Security;
    
    /// <summary>
    ///This is a test class for CosmoMongerMembershipProviderTest and is intended
    ///to contain all CosmoMongerMembershipProviderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CosmoMongerMembershipProviderTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for RequiresUniqueEmail
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void RequiresUniqueEmailTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RequiresUniqueEmail;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for RequiresQuestionAndAnswer
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void RequiresQuestionAndAnswerTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.RequiresQuestionAndAnswer;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PasswordStrengthRegularExpression
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void PasswordStrengthRegularExpressionTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string actual;
            actual = target.PasswordStrengthRegularExpression;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PasswordFormat
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void PasswordFormatTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            MembershipPasswordFormat actual;
            actual = target.PasswordFormat;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for PasswordAttemptWindow
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void PasswordAttemptWindowTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.PasswordAttemptWindow;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MinRequiredPasswordLength
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void MinRequiredPasswordLengthTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.MinRequiredPasswordLength;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MinRequiredNonAlphanumericCharacters
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void MinRequiredNonAlphanumericCharactersTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.MinRequiredNonAlphanumericCharacters;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MaxInvalidPasswordAttempts
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void MaxInvalidPasswordAttemptsTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.MaxInvalidPasswordAttempts;
            Assert.AreEqual(3, actual, "Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnablePasswordRetrieval
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void EnablePasswordRetrievalTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.EnablePasswordRetrieval;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for EnablePasswordReset
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void EnablePasswordResetTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.EnablePasswordReset;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ApplicationName
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void ApplicationNameTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.ApplicationName = expected;
            actual = target.ApplicationName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ValidateUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void ValidateUserTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ValidateUser(username, password);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for UpdateUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void UpdateUserTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            MembershipUser user = null; // TODO: Initialize to an appropriate value
            target.UpdateUser(user);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for UnlockUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void UnlockUserTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string userName = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.UnlockUser(userName);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ResetPassword
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void ResetPasswordTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            string answer = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ResetPassword(username, answer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUserNameByEmail
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void GetUserNameByEmailTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string email = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetUserNameByEmail(email);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void GetUserTest1()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            object providerUserKey = null; // TODO: Initialize to an appropriate value
            bool userIsOnline = false; // TODO: Initialize to an appropriate value
            MembershipUser expected = null; // TODO: Initialize to an appropriate value
            MembershipUser actual;
            actual = target.GetUser(providerUserKey, userIsOnline);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void GetUserTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            bool userIsOnline = false; // TODO: Initialize to an appropriate value
            MembershipUser expected = null; // TODO: Initialize to an appropriate value
            MembershipUser actual;
            actual = target.GetUser(username, userIsOnline);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetPassword
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void GetPasswordTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            string answer = string.Empty; // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.GetPassword(username, answer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetNumberOfUsersOnline
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void GetNumberOfUsersOnlineTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetNumberOfUsersOnline();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAllUsers
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void GetAllUsersTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            int pageIndex = 0; // TODO: Initialize to an appropriate value
            int pageSize = 0; // TODO: Initialize to an appropriate value
            int totalRecords = 0; // TODO: Initialize to an appropriate value
            int totalRecordsExpected = 0; // TODO: Initialize to an appropriate value
            MembershipUserCollection expected = null; // TODO: Initialize to an appropriate value
            MembershipUserCollection actual;
            actual = target.GetAllUsers(pageIndex, pageSize, out totalRecords);
            Assert.AreEqual(totalRecordsExpected, totalRecords);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindUsersByName
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void FindUsersByNameTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string usernameToMatch = string.Empty; // TODO: Initialize to an appropriate value
            int pageIndex = 0; // TODO: Initialize to an appropriate value
            int pageSize = 0; // TODO: Initialize to an appropriate value
            int totalRecords = 0; // TODO: Initialize to an appropriate value
            int totalRecordsExpected = 0; // TODO: Initialize to an appropriate value
            MembershipUserCollection expected = null; // TODO: Initialize to an appropriate value
            MembershipUserCollection actual;
            actual = target.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
            Assert.AreEqual(totalRecordsExpected, totalRecords);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for FindUsersByEmail
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void FindUsersByEmailTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string emailToMatch = string.Empty; // TODO: Initialize to an appropriate value
            int pageIndex = 0; // TODO: Initialize to an appropriate value
            int pageSize = 0; // TODO: Initialize to an appropriate value
            int totalRecords = 0; // TODO: Initialize to an appropriate value
            int totalRecordsExpected = 0; // TODO: Initialize to an appropriate value
            MembershipUserCollection expected = null; // TODO: Initialize to an appropriate value
            MembershipUserCollection actual;
            actual = target.FindUsersByEmail(emailToMatch, pageIndex, pageSize, out totalRecords);
            Assert.AreEqual(totalRecordsExpected, totalRecords);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for DeleteUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void DeleteUserTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            bool deleteAllRelatedData = false; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.DeleteUser(username, deleteAllRelatedData);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CreateUser
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void CreateUserTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            string email = string.Empty; // TODO: Initialize to an appropriate value
            string passwordQuestion = string.Empty; // TODO: Initialize to an appropriate value
            string passwordAnswer = string.Empty; // TODO: Initialize to an appropriate value
            bool isApproved = false; // TODO: Initialize to an appropriate value
            object providerUserKey = null; // TODO: Initialize to an appropriate value
            MembershipCreateStatus status = new MembershipCreateStatus(); // TODO: Initialize to an appropriate value
            MembershipCreateStatus statusExpected = new MembershipCreateStatus(); // TODO: Initialize to an appropriate value
            MembershipUser expected = null; // TODO: Initialize to an appropriate value
            MembershipUser actual;
            actual = target.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
            Assert.AreEqual(statusExpected, status);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ChangePasswordQuestionAndAnswer
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void ChangePasswordQuestionAndAnswerTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            string password = string.Empty; // TODO: Initialize to an appropriate value
            string newPasswordQuestion = string.Empty; // TODO: Initialize to an appropriate value
            string newPasswordAnswer = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ChangePassword
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void ChangePasswordTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider(); // TODO: Initialize to an appropriate value
            string username = string.Empty; // TODO: Initialize to an appropriate value
            string oldPassword = string.Empty; // TODO: Initialize to an appropriate value
            string newPassword = string.Empty; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.ChangePassword(username, oldPassword, newPassword);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for CosmoMongerMembershipProvider Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Jory\\Documents\\Visual Studio 2008\\Projects\\CosmoMonger\\CosmoMonger", "/")]
        [UrlToTest("http://localhost:54084/")]
        public void CosmoMongerMembershipProviderConstructorTest()
        {
            CosmoMongerMembershipProvider target = new CosmoMongerMembershipProvider();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
