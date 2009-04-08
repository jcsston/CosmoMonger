namespace CosmoMonger.Tests.Models.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Web.Security;
    using CosmoMonger.Models.Utility;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class ProbablityCalculatorTest
    {
        [Test]
        public void TestBool5050()
        {
            ProbablityCalculator p = new ProbablityCalculator();
            int trueCount = 0;
            int falseCount = 0;

            for (int i = 0; i < 1000; i++)
            {
                bool result = p.SelectByProbablity(new bool[] { true, false }, new double[] { 0.50, 0.50 });
                if (result)
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }

            Debug.WriteLine(string.Format("True: {0} False: {1}", trueCount, falseCount));
            Assert.That(Math.Abs(trueCount - falseCount), Is.LessThan(100), "1000 selections at 50/50 should give -/+10%");
        }

        [Test]
        public void TestBool2575()
        {
            ProbablityCalculator p = new ProbablityCalculator();
            int trueCount = 0;
            int falseCount = 0;

            for (int i = 0; i < 1000; i++)
            {
                bool result = p.SelectByProbablity(new bool[] { true, false }, new double[] { 0.25, 0.75 });
                if (result)
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }

            Debug.WriteLine(string.Format("True: {0} False: {1}", trueCount, falseCount));
            Assert.That(Math.Abs((trueCount * 3) - falseCount), Is.LessThan(200), "1000 selections at 25/75 should give -/+10%");
        }

        [Test]
        public void TestBool3366()
        {
            ProbablityCalculator p = new ProbablityCalculator();
            int trueCount = 0;
            int falseCount = 0;

            for (int i = 0; i < 1000; i++)
            {
                bool result = p.SelectByProbablity(new bool[] { true, false }, new double[] { 0.66, 0.33 });
                if (result)
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }

            Debug.WriteLine(string.Format("True: {0} False: {1}", trueCount, falseCount));
            Assert.That(Math.Abs(trueCount - (falseCount * 2)), Is.LessThan(200), "1000 selections at 66/33 should give -/+10%");
        }

        [Test]
        public void TestBool333333()
        {
            ProbablityCalculator p = new ProbablityCalculator();
            int trueCount = 0;
            int falseCount = 0;

            for (int i = 0; i < 1000; i++)
            {
                bool result = p.SelectByProbablity(new bool[] { true, false, true }, new double[] { 0.33, 0.33, 0.33 });
                if (result)
                {
                    trueCount++;
                }
                else
                {
                    falseCount++;
                }
            }

            Debug.WriteLine(string.Format("True: {0} False: {1}", trueCount, falseCount));
            Assert.That(Math.Abs(trueCount - (falseCount * 2)), Is.LessThan(200), "1000 selections at T/F/T 33/33/33 should give -/+10%");
        }
    }
}
