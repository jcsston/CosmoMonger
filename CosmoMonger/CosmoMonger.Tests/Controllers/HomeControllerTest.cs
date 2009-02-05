namespace CosmoMonger.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using CosmoMonger;
    using CosmoMonger.Controllers;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using System.IO;
    using System.Web;
    using System.Web.Routing;
    using Moq.Mvc;
    using Moq;

    /// <summary>
    /// Summary description for HomeControllerTest
    /// </summary>
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void TestDefaultRoute()
        {
            // Arrange
            // Mock the HTTP request also
            HttpRequestMock mockRequest = new HttpRequestMock();
            Uri mockUrl = new Uri("http://www.cosmomonger.com/");
            mockRequest.Expect(r => r.Url)
                .Returns(mockUrl);
            mockRequest.Expect(r => r.HttpMethod)
                .Returns("GET");
            mockRequest.Expect(r => r.AppRelativeCurrentExecutionFilePath)
                .Returns("~/");

            HttpContextMock mockHttpContext = new HttpContextMock();
            mockHttpContext.Expect(c => c.Request)
                .Returns(mockRequest.Object);

            // Act
            RouteCollection routeCollection = new RouteCollection();
            MvcApplication.RegisterRoutes(routeCollection);
            RouteData routeData = routeCollection.GetRouteData(mockHttpContext.Object);

            // Assert
            Assert.AreEqual("Home", routeData.Values["controller"], "Default controller is HomeController");
            Assert.AreEqual("Index", routeData.Values["action"], "Default action is Index");
            Assert.AreEqual(String.Empty, routeData.Values["id"], "Default Id is empty string");

            /*
            // This code can almost render a view
            HttpRequestMock mockRequest = new HttpRequestMock();
            Uri mockUrl = new Uri("http://www.cosmomonger.com/");
            mockRequest.Expect(r => r.Url)
                .Returns(mockUrl);
            mockRequest.Expect(r => r.HttpMethod)
                .Returns("GET");
            mockRequest.Expect(r => r.AppRelativeCurrentExecutionFilePath)
                .Returns("~/");

            HttpContextMock mockHttpContext = new HttpContextMock();
            mockHttpContext.Expect(c => c.Request)
                .Returns(mockRequest.Object);

            RouteCollection routeCollection = new RouteCollection();
            MvcApplication.RegisterRoutes(routeCollection);
            RouteData routeData = routeCollection.GetRouteData(mockHttpContext.Object);

            ControllerContext controllerContext = new ControllerContext(mockHttpContext.Object, routeData, controller);
            StringWriter writer = new StringWriter();
            result.ExecuteResult(controllerContext);
            ViewContext viewContext = new ViewContext(controllerContext, result.View, result.ViewData, result.TempData);
            result.View.Render(viewContext, writer);
            Assert.Fail(writer.ToString());
            */
        }

        [Test]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            ViewDataDictionary viewData = result.ViewData;
            Assert.That(result.ViewName, Is.EqualTo(""), "The correct view is returned");
        }

        [Test]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            ViewDataDictionary viewData = result.ViewData;
            Assert.That(result.ViewName, Is.EqualTo(""), "The correct view is returned");
        }
    }
}
