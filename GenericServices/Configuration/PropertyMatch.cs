// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.Internal;

namespace GenericServices.Configuration
{
    /// <summary>
    /// This represents source of the data to go into a method/ctor. It to allow injection of DbContext into access methods
    /// </summary>
    public enum MatchSources { Property, DbContext }
    
    /// <summary>
    /// This holds the information on a match between a name/type and a propertyInfo, with a score to set how it did
    /// This is used by a name matcher to try to match method/ctor properties to a set of properties in a class
    /// </summary>
    public class PropertyMatch
    {
        public const double PerfectMatchValue = 0.99999;
        public const double NoMatchAtAll = 0.00001;

        public enum TypeMatchLevels
        {
            NoMatch = 0, NeedsCast = 1, NeedsCastOnCollection = 2, Match = 3
        }

        public bool NameMatched { get; }
        public TypeMatchLevels TypeMatch { get; }

        public PropertyInfo PropertyInfo { get; }

        public MatchSources MatchSource { get; }

        /// <summary>
        /// A Score of 1 means a perfect match. 
        /// </summary>
        public double Score => (NameMatched ? 0.7 : 0.0) + ((int) TypeMatch / 10.0);

        public PropertyMatch(bool nameMatched, TypeMatchLevels typeMatch, PropertyInfo propertyInfo, MatchSources matchSource = MatchSources.Property)
        {
            NameMatched = nameMatched;
            TypeMatch = typeMatch; 
            if ((int)TypeMatch > 3)
                throw new InvalidOperationException("The TypeMatchLevels must run from 0 to 3, 3 being a perfect match.");
            PropertyInfo = propertyInfo;
            MatchSource = matchSource;
        }

        public override string ToString()
        {
            string matchInfo = "wrong name";
            if (Score >= PropertyMatch.PerfectMatchValue)
                matchInfo = $"{PropertyInfo.PropertyType.Name} {PropertyInfo.Name}";
            else if (Score <= NoMatchAtAll)
                matchInfo = "nothing matches";
            else if (!NameMatched)
                matchInfo = "Name not match, but type is " + TypeMatch.ToString().SplitPascalCase();

            return matchInfo;
        }
    }
}