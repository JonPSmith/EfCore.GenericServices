// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericServices.Configuration;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.Decoders
{
    /// <summary>
    /// This will match a set of method/ctor parameters against a set of possible properties.
    /// It will return a set of PropertyMatchs that are the best fit againts the parameters it has to work with
    /// </summary>
    internal class ParametersMatch
    {
        /// <summary>
        /// This holds the matching properties to the method/ctor in order. If there is no match the entry will be null
        /// </summary>
        public ImmutableList<PropertyMatch> MatchedPropertiesInOrder { get; }

        public double Score => MatchedPropertiesInOrder.Any()
            ? MatchedPropertiesInOrder.Average(x => x?.Score ?? 0)
            : 1;  //if there are no parameters then it is a perfect fit!

        public ParametersMatch(IEnumerable<ParameterInfo> parameters, List<PropertyInfo> propertiesToMatch, MatchNameAndType propMatcher)
        {
            var matchedProps = new List<PropertyMatch>();
            foreach (var parameter in parameters)
            {
                matchedProps.Add(FindMatch(parameter, propertiesToMatch, propMatcher));
            }

            MatchedPropertiesInOrder = matchedProps.ToImmutableList();
        }

        private static PropertyMatch FindMatch(ParameterInfo parameter, IEnumerable<PropertyInfo> propertiesToCheck, MatchNameAndType propMatcher)
        {
            PropertyMatch bestMatch = null;
            foreach (var propertyInfo in propertiesToCheck)
            {
                var match = propMatcher(parameter.Name, parameter.ParameterType, propertyInfo);
                if (bestMatch == null || bestMatch.Score < match.Score)
                    bestMatch = match;
            }
            return bestMatch;
        }

    }
}