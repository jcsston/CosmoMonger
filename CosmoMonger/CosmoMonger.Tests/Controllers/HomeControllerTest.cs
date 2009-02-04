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
        //[Test]
        public void TestDefaultRoute()
        {
            // Arrange
            RouteCollection routes = new RouteCollection();
            CosmoMonger.MvcApplication.RegisterRoutes(routes);

            // Act
            HttpContextMock context = new HttpContextMock();
            context.HttpRequest.Expect(r => r.HttpMethod).Returns("GET");
            context.HttpRequest.Expect(r => r.Path).Returns("~/");
            RouteData routeData = routes.GetRouteData(context.Object);

            // Assert
            Assert.AreEqual("Home", routeData.Values["controller"], "Default controller is HomeController");
            Assert.AreEqual("Index", routeData.Values["action"], "Default action is Index");
            Assert.AreEqual(String.Empty, routeData.Values["id"], "Default Id is empty string");
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

            /*
            HttpContextMock httpContext = new HttpContextMock();
            RouteData routeData = new RouteData();
            StringWriter writer = new StringWriter();
            ViewContext viewContext = new ViewContext(httpContext.Object, routeData, controller, result.View, result.ViewData, result.TempData);
            result.View.Render(viewContext, writer);
             * */
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
