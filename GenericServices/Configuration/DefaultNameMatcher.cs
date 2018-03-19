// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;

namespace GenericServices.Configuration
{
    public static class DefaultNameMatcher
    {
        public static PropertyMatch MatchCamelAndPascalName(string name, Type type, PropertyInfo propertyInfo)
        {
            //The first item could be a method name, which starts with a lower case
            var nameMatched = name.FirstCharToUpper() == propertyInfo.Name;
            //I have only done a simple match - someone can do a better match for collections etc.
            var typeMatch = type == propertyInfo.PropertyType
                ? PropertyMatch.TypeMatchLevels.PerfectMatch
                : PropertyMatch.TypeMatchLevels.NoMatch;
            return new PropertyMatch(nameMatched, typeMatch, propertyInfo);
        }

        //thanks to https://stackoverflow.com/questions/3565015/bestpractice-transform-first-character-of-a-string-into-lower-case
        private static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}