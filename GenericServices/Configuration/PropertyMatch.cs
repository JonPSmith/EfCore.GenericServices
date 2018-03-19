// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.Internal;

namespace GenericServices.Configuration
{
    
    public struct PropertyMatch
    {
        public const double PerfectMatchValue = 0.99999;
        public const double NoMatchAtAll = 0.00001;

        public PropertyMatch(bool nameMatched, TypeMatchLevels typeMatch, PropertyInfo propertyInfo) : this()
        {
            NameMatched = nameMatched;
            TypeMatch = typeMatch; 
            if ((int)TypeMatch > 3)
                throw new InvalidOperationException("The TypeMatchLevels must run from 0 to 3, 3 being a perfect match.");
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
        }

        public enum TypeMatchLevels
        {
            NoMatch = 0, NeedsCast = 1, NeedsCastOnCollection = 2, PerfectMatch = 3
        }

        public bool NameMatched { get; private set; }
        public TypeMatchLevels TypeMatch { get; private set; }

        public PropertyInfo PropertyInfo { get; private set; }

        /// <summary>
        /// A Score of 1 means a perfect match. 
        /// </summary>
        public double Score => (NameMatched ? 0.7 : 0.0) + ((int) TypeMatch / 10.0);

        public override string ToString()
        {
            string matchInfo = "wrong name";
            if (Score >= PropertyMatch.PerfectMatchValue)
                matchInfo = "MATCH";
            else if (Score <= NoMatchAtAll)
                matchInfo = "Neither name or type matches";
            else if (!NameMatched)
                matchInfo = "Name not match, but type is " + TypeMatch.ToString().SplitPascalCase();

            return $"{PropertyInfo.PropertyType.Name} {PropertyInfo.Name} ({matchInfo})";
        }
    }
}