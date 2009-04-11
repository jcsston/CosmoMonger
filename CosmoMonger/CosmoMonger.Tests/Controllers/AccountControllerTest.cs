
namespace CosmoMonger.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using System.Web.Security;
    using CosmoMonger.Controllers;
    using CosmoMonger.Models;
    using Moq;
    using Moq.Mvc;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class AccountControllerTest
    {
        [Test]
        public void LoginSuccessful()
        {
            string testUserName = "TestUser";
            string testPassword = "TestPassword";

            // Setup the mock objects
            Mock<CosmoMongerMembershipProvider> mockMembership = new Mock<CosmoMongerMembershipProvider>();
            Mock<User> mockUserModel = new Mock<User>();
            Mock<CosmoMongerMembershipUser> mockUser = new Mock<CosmoMongerMembershipUser>(mockUserModel.Object);
            mockMembership.Expect<MembershipUser>(m => m.GetUser(testUserName, true))
                .Returns(mockUser.Object).AtMostOnce().Verifiable();

            mockMembership.Expect<bool>(m => m.ValidateUser(testUserName, testPassword))
               .Returns(true).AtMostOnce().Verifiable();
            AccountController controller = new AccountController(mockMembership.Object);
            ActionResult result = controller.Login(testUserName, testPassword, "");

            Assert.That(result, Is.TypeOf(typeof(FormsLoginResult)));

            FormsLoginResult loginResult = (FormsLoginResult)result;
            Assert.That(loginResult.UserName, Is.EqualTo(testUserName));
            Assert.That(loginResult.PersistentCookie, Is.True);
        }

        [Test]
        public void LoginFailure()
        {
            string testUserName = "TestUser";
            string testPassword = "TestPassword";

            Mock<CosmoMongerMembershipProvider> mockMembership = new Mock<CosmoMongerMembershipProvider>();
            Mock<User> mockUserModel = new Mock<User>();
            Mock<CosmoMongerMembershipUser> mockUser = new Mock<CosmoMongerMembershipUser>(mockUserModel.Object);
            mockMembership.Expect<MembershipUser>(m => m.GetUser(testUserName, true))
                .Returns(mockUser.Object).AtMostOnce().Verifiable();
            mockMembership.Expect<bool>(m => m.ValidateUser(testUserName, testPassword))
               .Returns(true).AtMostOnce().Verifiable();

            AccountController controller = new AccountController(mockMembership.Object);
            ActionResult result = controller.Login(testUserName, "badPassword", "");

            Assert.That(result, Is.TypeOf(typeof(ViewResult)));
        }

        [Test]
        [Ignore("This test is not complete yet")]
        public void SendVerificationCodeSuccessful()
        {
            // Arrange
            string testUserName = "TestUser";

            Mock<User> mockUserModel = new Mock<User>();
            Mock<CosmoMongerMembershipUser> mockUser = new Mock<CosmoMongerMembershipUser>(mockUserModel.Object);
            mockUser.Expect(m => m.SendVerificationCode(It.IsRegex("http://www.cosmomonger.com/Account/VerifyEmail?username=TestUser&verificationCode=.*")))
               .AtMostOnce().Verifiable();

            Mock<MembershipProvider> mockMembership = new Mock<MembershipProvider>();
            mockMembership.Expect<MembershipUser>(m => m.GetUser(testUserName, false))
                .Returns(mockUser.Object).AtMostOnce().Verifiable();

            // Mock the HTTP request also
            HttpRequestMock mockRequest = new HttpRequestMock();
            Uri mockUrl = new Uri("http://www.cosmomonger.com/Account/SendVerificationCode?username=TestUser");
            mockRequest.Expect(r => r.Url)
                .Returns(mockUrl);
            mockRequest.Expect(r => r.HttpMethod)
                .Returns("GET");
            mockRequest.Expect(r => r.AppRelativeCurrentExecutionFilePath)
                .Returns("~/Account/");
            
            HttpContextMock mockHttpContext = new HttpContextMock();
            mockHttpContext.Expect(c => c.Request)
                .Returns(mockRequest.Object);

            RouteCollection routeCollection = new RouteCollection();
            MvcApplication.RegisterRoutes(routeCollection);
            RouteData route = routeCollection.GetRouteData(mockHttpContext.Object);

            AccountController controller = new AccountController(mockMembership.Object);
            controller.ControllerContext = new ControllerContext(mockHttpContext.Object, route, new Mock<ControllerBase>().Object);
            controller.Url = new UrlHelper(new RequestContext(mockHttpContext.Object, route));
            
            // Act
            ViewResult result = (ViewResult)controller.SendVerificationCode(testUserName);

            // Assert
            Assert.That(result.ViewName, Is.EqualTo("SentVerificationCode"), "Should have returned the SentVerificationCode view");
        }

        [Test]
        public void ChangePasswordGetReturnsView()
        {
            // Arrange
            Mock<CosmoMongerMembershipProvider> mockMembership = new Mock<CosmoMongerMembershipProvider>();
            AccountController controller = new AccountController(mockMembership.Object);

            // Act
            ViewResult result = (ViewResult)controller.ChangePassword();

            // Assert
            Assert.That(result.ViewData.ModelState.IsValid, "No errors should be returned");
        }

        [Test]
        public void ChangePasswordPostRedirectsOnSuccess()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            RedirectToRouteResult result = (RedirectToRouteResult)controller.ChangePassword("oldPass", "newPass", "newPass");

            // Assert
            Assert.AreEqual("ChangePasswordSuccess", result.RouteValues["action"]);
        }

        [Test]
        public void ChangePasswordPostReturnsViewIfCurrentPasswordNotSpecified()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.ChangePassword("", "newPassword", "newPassword");

            // Assert
            Assert.AreEqual("You must specify a current password.", result.ViewData.ModelState["currentPassword"].Errors[0].ErrorMessage);
        }

        [Test]
        public void ChangePasswordPostReturnsViewIfNewPasswordDoesNotMatchConfirmPassword()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.ChangePassword("currentPassword", "newPassword", "otherPassword");

            // Assert
            Assert.AreEqual("The new password and confirmation password do not match.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [Test]
        public void ChangePasswordPostReturnsViewIfNewPasswordIsNull()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.ChangePassword("currentPassword", null, null);

            // Assert
            Assert.AreEqual("You must specify a new password of 6 or more characters.", result.ViewData.ModelState["newPassword"].Errors[0].ErrorMessage);
        }

        [Test]
        public void ChangePasswordPostReturnsViewIfNewPasswordIsTooShort()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.ChangePassword("currentPassword", "12345", "12345");

            // Assert
            Assert.AreEqual("You must specify a new password of 6 or more characters.", result.ViewData.ModelState["newPassword"].Errors[0].ErrorMessage);
        }

        [Test]
        public void ChangePasswordPostReturnsViewIfProviderRejectsPassword()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.ChangePassword("oldPass", "badPass", "badPass");

            // Assert
            Assert.AreEqual("The current password is incorrect or the new password is invalid.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [Test]
        public void ChangePasswordSuccess()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.ChangePasswordSuccess();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void ConstructorSetsProperties()
        {
            // Arrange
            Mock<MembershipProvider> memebershipProvider = new Mock<MembershipProvider>();

            // Act
            AccountController controller = new AccountController(memebershipProvider.Object);

            // Assert
            Assert.AreEqual(memebershipProvider.Object, controller.Provider, "Provider property did not match.");
        }

        [Test]
        public void ConstructorSetsPropertiesToDefaultValues()
        {
            // Act
            AccountController controller = new AccountController();

            // Assert
            Assert.IsNotNull(controller.Provider, "Provider property is null.");
        }

        [Test]
        public void LoginGet()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Login();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void LoginPostRedirectsHomeIfLoginSuccessfulButNoReturnUrlGiven()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            FormsLoginResult result = (FormsLoginResult)controller.Login("someUser", "goodPass", null);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void LoginPostRedirectsToReturnUrlIfLoginSuccessfulAndReturnUrlGiven()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            FormsLoginResult result = (FormsLoginResult)controller.Login("someUser", "goodPass", "someUrl");

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void LoginPostReturnsViewIfPasswordNotSpecified()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Login("username", "", null);

            // Assert
            Assert.AreEqual("You must specify a password.", result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [Test]
        public void LoginPostReturnsViewIfUsernameNotSpecified()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Login("", "somePass", null);

            // Assert
            Assert.AreEqual("You must specify a username.", result.ViewData.ModelState["username"].Errors[0].ErrorMessage);
        }

        [Test]
        public void LoginPostReturnsViewIfUsernameIsIncorrect()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Login("badUser", "badPass", null);

            // Assert
            Assert.That(result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void LoginPostReturnsViewIfPasswordIsIncorrect()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Login("someUser", "badPass", null);

            // Assert
            Assert.That(result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage, Is.Not.Null);
        }

        [Test]
        public void LogOff()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            FormsLogoutResult result = (FormsLogoutResult)controller.Logout();
            
            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void RegisterGet()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register();

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        [Ignore("Need to mock Request object")]
        public void RegisterPostRedirectsHomeIfRegistrationSuccessful()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            RedirectToRouteResult result = (RedirectToRouteResult)controller.Register("someUser", "email", "goodPass", "goodPass");

            // Assert
            Assert.AreEqual("Home", result.RouteValues["controller"]);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [Test]
        public void RegisterPostReturnsViewIfEmailNotSpecified()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register("username", "", "password", "password");

            // Assert
            Assert.AreEqual("You must specify an email address.", result.ViewData.ModelState["email"].Errors[0].ErrorMessage);
        }

        [Test]
        public void RegisterPostReturnsViewIfNewPasswordDoesNotMatchConfirmPassword()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register("username", "email", "password", "password2");

            // Assert
            Assert.AreEqual("The new password and confirmation password do not match.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [Test]
        public void RegisterPostReturnsViewIfPasswordIsNull()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register("username", "email", null, null);

            // Assert
            Assert.AreEqual("You must specify a new password of 6 or more characters.", result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [Test]
        public void RegisterPostReturnsViewIfPasswordIsTooShort()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register("username", "email", "12345", "12345");

            // Assert
            Assert.AreEqual("You must specify a new password of 6 or more characters.", result.ViewData.ModelState["password"].Errors[0].ErrorMessage);
        }

        [Test]
        [Ignore("Need to mock Request object")]
        public void RegisterPostReturnsViewIfRegistrationFails()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register("someUser", "DuplicateUserName" /* error */, "badPass", "badPass");

            // Assert
            Assert.AreEqual("Username already exists. Please enter a different user name.", result.ViewData.ModelState["_FORM"].Errors[0].ErrorMessage);
        }

        [Test]
        public void RegisterPostReturnsViewIfUsernameNotSpecified()
        {
            // Arrange
            AccountController controller = GetAccountController();

            // Act
            ViewResult result = (ViewResult)controller.Register("", "email", "password", "password");

            // Assert
            Assert.AreEqual("You must specify a username.", result.ViewData.ModelState["username"].Errors[0].ErrorMessage);
        }

        private static AccountController GetAccountController()
        {
            MembershipProvider membershipProvider = new MockMembershipProvider();
            AccountController controller = new AccountController(membershipProvider);
            ControllerContext controllerContext = new ControllerContext(new MockHttpContext(), new RouteData(), controller);
            controller.ControllerContext = controllerContext;
            return controller;
        }

        public class MockIdentity : IIdentity
        {
            public string AuthenticationType
            {
                get
                {
                    return "MockAuthentication";
                }
            }

            public bool IsAuthenticated
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "someUser";
                }
            }
        }

        public class MockPrincipal : IPrincipal
        {
            IIdentity _identity;

            public IIdentity Identity
            {
                get
                {
                    if (_identity == null)
                    {
                        _identity = new MockIdentity();
                    }
                    return _identity;
                }
            }

            public bool IsInRole(string role)
            {
                return false;
            }
        }

        public class MockMembershipUser : CosmoMongerMembershipUser
        {
            public MockMembershipUser()
                : base(new Mock<User>().Object)
            {
            }

            public override bool ChangePassword(string oldPassword, string newPassword)
            {
                return newPassword.Equals("newPass");
            }
        }

        public class MockHttpContext : HttpContextBase
        {
            private IPrincipal _user;
            private Mock<HttpSessionStateBase> _session;
            private Mock<HttpRequestBase> _request;

            public override IPrincipal User
            {
                get
                {
                    if (_user == null)
                    {
                        _user = new MockPrincipal();
                    }
                    return _user;
                }
                set
                {
                    _user = value;
                }
            }

            public override HttpRequestBase Request
            {
                get
                {
                    if (_request == null)
                    {
                        _request = new Mock<HttpRequestBase>();
                    }
                    return _request.Object;
                }
            }

            public override HttpSessionStateBase Session
            {
                get
                {
                    if (_session == null)
                    {
                        _session = new Mock<HttpSessionStateBase>();
                    }
                    return _session.Object;
                }
            }
        }

        public class MockMembershipProvider : CosmoMongerMembershipProvider
        {
            string _applicationName;

            public override string ApplicationName
            {
                get
                {
                    return _applicationName;
                }
                set
                {
                    _applicationName = value;
                }
            }

            public override bool EnablePasswordReset
            {
                get
                {
                    return false;
                }
            }

            public override bool EnablePasswordRetrieval
            {
                get
                {
                    return false;
                }
            }

            public override int MaxInvalidPasswordAttempts
            {
                get
                {
                    return 0;
                }
            }

            public override int MinRequiredNonAlphanumericCharacters
            {
                get
                {
                    return 0;
                }
            }

            public override int MinRequiredPasswordLength
            {
                get
                {
                    return 6;
                }
            }

            public override string Name
            {
                get
                {
                    return null;
                }
            }

            public override int PasswordAttemptWindow
            {
                get
                {
                    return 3;
                }
            }

            public override MembershipPasswordFormat PasswordFormat
            {
                get
                {
                    return MembershipPasswordFormat.Clear;
                }
            }

            public override string PasswordStrengthRegularExpression
            {
                get
                {
                    return null;
                }
            }

            public override bool RequiresQuestionAndAnswer
            {
                get
                {
                    return false;
                }
            }

            public override bool RequiresUniqueEmail
            {
                get
                {
                    return false;
                }
            }

            public override bool ChangePassword(string username, string oldPassword, string newPassword)
            {
                return username.Equals("someUser") && oldPassword.Equals("oldPass") && newPassword.Equals("newPass");
            }

            public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, Object providerUserKey, out MembershipCreateStatus status)
            {
                MockMembershipUser user = new MockMembershipUser();

                if (username.Equals("someUser") && password.Equals("goodPass") && email.Equals("email"))
                {
                    status = MembershipCreateStatus.Success;
                }
                else
                {
                    // the 'email' parameter contains the status we want to return to the user
                    status = (MembershipCreateStatus)Enum.Parse(typeof(MembershipCreateStatus), email);
                }

                return user;
            }

            public override bool DeleteUser(string username, bool deleteAllRelatedData)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
            {
                throw new NotImplementedException();
            }

            public override int GetNumberOfUsersOnline()
            {
                throw new NotImplementedException();
            }

            public override string GetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override string GetUserNameByEmail(string email)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(Object providerUserKey, bool userIsOnline)
            {
                throw new NotImplementedException();
            }

            public override MembershipUser GetUser(string username, bool userIsOnline)
            {
                return new MockMembershipUser();
            }

            public override string ResetPassword(string username, string answer)
            {
                throw new NotImplementedException();
            }

            public override bool UnlockUser(string userName)
            {
                throw new NotImplementedException();
            }

            public override void UpdateUser(MembershipUser user)
            {
                throw new NotImplementedException();
            }

            public override bool ValidateUser(string username, string password)
            {
                return password.Equals("goodPass");
            }
        }
    }
}
