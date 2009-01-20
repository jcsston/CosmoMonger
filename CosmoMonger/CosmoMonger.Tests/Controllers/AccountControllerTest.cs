
namespace CosmoMonger.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using Moq;
    using CosmoMonger.Controllers;
    using System.Web.Mvc;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using CosmoMonger.Models;
    using Moq.Mvc;
    using System.Web.Routing;

    [TestFixture]
    public class AccountControllerTest
    {
        [Test]
        public void LoginSuccessful()
        {
            string testUserName = "TestUser";
            string testPassword = "TestPassword";

            // Setup the mock objects
            Mock<MembershipProvider> mockMembership = new Mock<MembershipProvider>();
            Mock<MembershipUser> mockUser = new Mock<MembershipUser>();
            mockMembership.Expect<MembershipUser>(m => m.GetUser(testUserName, true))
                .Returns(mockUser.Object).AtMostOnce().Verifiable();

            mockMembership.Expect<bool>(m => m.ValidateUser(testUserName, testPassword))
               .Returns(true).AtMostOnce().Verifiable();

            AccountController controller = new AccountController(mockMembership.Object);
            ActionResult result = controller.Login(testUserName, testPassword, false, "");

            Assert.That(result, Is.TypeOf(typeof(FormsLoginResult)));

            FormsLoginResult loginResult = (FormsLoginResult)result;
            Assert.That(loginResult.UserName, Is.EqualTo(testUserName));
            Assert.That(loginResult.PersistentCookie, Is.False);
        }

        [Test]
        public void LoginFailure()
        {
            string testUserName = "TestUser";
            string testPassword = "TestPassword";

            Mock<MembershipProvider> mockMembership = new Mock<MembershipProvider>();
            Mock<MembershipUser> mockUser = new Mock<MembershipUser>();
            mockMembership.Expect<MembershipUser>(m => m.GetUser(testUserName, true))
                .Returns(mockUser.Object).AtMostOnce().Verifiable();
            mockMembership.Expect<bool>(m => m.ValidateUser(testUserName, testPassword))
               .Returns(true).AtMostOnce().Verifiable();

            AccountController controller = new AccountController(mockMembership.Object);
            ActionResult result = controller.Login(testUserName, "badPassword", false, "");

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
    }
}
