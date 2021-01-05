// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.Decoders
{
    internal class BestPropertyMatch
    {
        public BestPropertyMatch(string name, Type type, PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            
            //This matches camel and Pascal names with some degree of match       
            Score = name.SplitPascalCase().Equals( propertyInfo?.Name.SplitPascalCase(), StringComparison.InvariantCultureIgnoreCase) ? 0.7 : 0;
            //The name is a higher match, as collection types can differ
            Score += type == propertyInfo?.PropertyType ? 0.3 : 0;
        }

        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// A Score of 1 means a perfect match. 
        /// </summary>
        public double Score { get; set; }

        public static BestPropertyMatch FindMatch(PropertyInfo propertyToMatch, IEnumerable<PropertyInfo> propertiesToCheck)
        {
            return FindMatch(propertyToMatch.Name, propertyToMatch.PropertyType, propertiesToCheck);
        }

        public static BestPropertyMatch FindMatch(string name, Type type, IEnumerable<PropertyInfo> propertiesToCheck)
        {
            if (propertiesToCheck == null)
                return null;
            BestPropertyMatch bestMatch = null;
            foreach (var propertyInfo in propertiesToCheck)
            {
                var match = new BestPropertyMatch(name, type, propertyInfo);
                if (bestMatch == null || bestMatch.Score < match.Score)
                    bestMatch = match;
            }

            return bestMatch;
        }
    }
}