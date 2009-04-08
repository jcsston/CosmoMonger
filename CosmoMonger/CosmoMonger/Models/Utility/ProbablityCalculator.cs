//-----------------------------------------------------------------------
// <copyright file="ProbablityCalculator.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    public class ProbablityCalculator : Random
    {
        public T SelectByProbablity<T>(T[] values, double[] probablities)
        {
            Debug.Assert(values.Length == probablities.Length, "Number of values should match number of probablities");
            Debug.Assert(values.Length > 1, "Number of values should be greater than one");
            Debug.Assert(probablities.Sum() <= 1.0, "Sum of probablities should be less or equal to 1.0");

            double r = this.NextDouble();
            double currentProbablity = 0;
            T value = default(T);
            for (int i = 0; i < values.Length; i++)
            {
                value = values[i];
                currentProbablity += probablities[i];

                if (r <= currentProbablity)
                {
                    break;
                }
            }

            return value;
        }

        public T SelectOne<T>(IEnumerable<T> values)
        {
            int selectedIndex = this.Next(values.Count());
            return values.Skip(selectedIndex).FirstOrDefault();
        }
    }
}
