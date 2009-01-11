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

    /// <summary>
    /// Summary description for HomeControllerTest
    /// </summary>
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            ViewDataDictionary viewData = result.ViewData;
            Assert.That(viewData["Title"], Is.EqualTo("CosmoMonger"), "The page title should match");
            //Assert.That(result.ViewName, Is.EqualTo("Index"), "The correct view is returned");
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
            Assert.That(viewData["Title"], Is.EqualTo("About CosmoMonger"));
            //Assert.That(result.ViewName, Is.EqualTo("About"));
        }
    }
}
