namespace CosmoMonger.Tests.Models.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CosmoMonger.Models.Utility;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class FormatTimeSpanTest
    {
        [Test]
        public void JustNow()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(0, 0, 1));
            Assert.That(format, Is.EqualTo("Just Now"), "Format string should match");
        }

        [Test]
        public void FiveMinutes()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(0, 5, 0));
            Assert.That(format, Is.EqualTo("5 Minutes Ago"), "Format string should match");
        }

        [Test]
        public void TwentyTwentyMinutes()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(0, 22, 0));
            Assert.That(format, Is.EqualTo("22 Minutes Ago"), "Format string should match");
        }

        [Test]
        public void OneHour()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(1, 0, 0));
            Assert.That(format, Is.EqualTo("1 Hour Ago"), "Format string should match");
        }

        [Test]
        public void TwoHours()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(2, 0, 0));
            Assert.That(format, Is.EqualTo("2 Hours Ago"), "Format string should match");
        }

        [Test]
        public void OneDay()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(1, 0, 0, 0));
            Assert.That(format, Is.EqualTo("1 Day Ago"), "Format string should match");
        }

        [Test]
        public void TwoDays()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(2, 0, 0, 0));
            Assert.That(format, Is.EqualTo("2 Days Ago"), "Format string should match");
        }

        [Test]
        public void FourWeeks()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(28, 0, 0, 0));
            Assert.That(format, Is.EqualTo("4 Weeks Ago"), "Format string should match");
        }

        [Test]
        public void OneYear()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(365, 0, 0, 0));
            Assert.That(format, Is.EqualTo("1 Year Ago"), "Format string should match");
        }

        [Test]
        public void ThreeYears()
        {
            string format = FormatTimeSpan.HumaneFormat(new TimeSpan(365*4, 0, 0, 0));
            Assert.That(format, Is.EqualTo("3 Years Ago"), "Format string should match");
        }
    }
}
