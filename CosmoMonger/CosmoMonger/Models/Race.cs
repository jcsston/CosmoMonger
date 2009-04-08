//-----------------------------------------------------------------------
// <copyright file="Race.cs" company="CosmoMonger">
//     Copyright (c) 2009 CosmoMonger. All rights reserved.
// </copyright>
// <author>Jory Stone</author>
//-----------------------------------------------------------------------
namespace CosmoMonger.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FastDynamicPropertyAccessor;

    /// <summary>
    /// Extenstion of the LINQ class Race
    /// </summary>
    public partial class Race
    {
        /// <summary>
        /// List of the properties that are racial modifiers
        /// </summary>
        public static string[] RacialModifiers = { "Accuracy", "Engine", "Shields", "Weapons" };

        /// <summary>
        /// Gets a Dictionary of Plus, Netural, and Minus keys containing a list of the racial modifiers.
        /// </summary>
        /// <returns>A Dictionary object with Plus, Netural, and Minus keys.</returns>
        public virtual Dictionary<string, List<string>> GetRacialModifiers()
        {     
            List<string> plusModifiers = new List<string>();
            List<string> neturalModifiers = new List<string>();
            List<string> minusModifiers = new List<string>();

            foreach (string racialModifier in Race.RacialModifiers)
            {
                // Access the racial properties by string name
                PropertyAccessor prop = new PropertyAccessor(typeof(Race), racialModifier);
                int value = (int)prop.Get(this);

                // Which category is the value?
                if (value > 0)
                {
                    plusModifiers.Add(racialModifier);
                }
                else if (value < 0)
                {
                    minusModifiers.Add(racialModifier);
                }
                else
                {
                    neturalModifiers.Add(racialModifier);
                }
            }

            return new Dictionary<string, List<string>>
            {
                { "Plus", plusModifiers },
                { "Netural", neturalModifiers },
                { "Minus", minusModifiers }
            };
        }
    }
}
