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
    internal enum HowTheyWereAskedFor { Unset, DefaultMatchToProperties, NamedMethodFromDtoClass, SpecifiedInPerDtoConfig}

    /// <summary>
    /// This is given a set of methodsOrCtors (with the same name) and picks the best matching method 
    /// that fits the set of possible properties that are available
    /// </summary>
    internal class MethodMatch
    {
        public MethodInfo Method { get;}
        public ConstructorInfo Constructor { get; }

        public ParametersMatch PropertiesMatch { get;  }

        public HowTheyWereAskedFor HowDefined { get; }

        public bool IsParameterlessMethod => !PropertiesMatch.MatchedPropertiesInOrder.Any();

        public MethodMatch(dynamic methodOrCtor, ParametersMatch propertiesMatch, HowTheyWereAskedFor howDefined)
        {
            Method = methodOrCtor as MethodInfo;
            Constructor = methodOrCtor as ConstructorInfo;
            PropertiesMatch = propertiesMatch ?? throw new ArgumentNullException(nameof(propertiesMatch));
            HowDefined = howDefined;
        }

        /// <summary>
        /// This takes a set of methods or Ctors and grades them by how well they fit to the parameters available in the DTO
        /// </summary>
        /// <param name="methodsOrCtors"></param>
        /// <param name="propertiesToCheck"></param>
        /// <param name="propMatcher"></param>
        /// <param name="howDefined"></param>
        /// <returns>It returns a collection of MethodMatch, with the best scores first, with secondary sort order with longest number of params first</returns>
        public static IEnumerable<MethodMatch> GradeAllMethods(dynamic[] methodsOrCtors, 
            List<PropertyInfo> propertiesToCheck, HowTheyWereAskedFor howDefined, MatchNameAndType propMatcher)
        {
            var result = methodsOrCtors.Select(method => new MethodMatch(method, 
                new ParametersMatch(method.GetParameters(), propertiesToCheck, propMatcher), howDefined));

            return result.OrderByDescending(x => x.PropertiesMatch.Score)
                .ThenByDescending(x => x.PropertiesMatch.MatchedPropertiesInOrder.Count);
        }

        public override string ToString()
        {
            var start = PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue
                ? "Match: "
                : $"Matched {PropertiesMatch.MatchedPropertiesInOrder.Count(x => x != null && x.Score >= PropertyMatch.NoMatchAtAll)}"+
                  $" params out of {PropertiesMatch.MatchedPropertiesInOrder.Count}. Score {PropertiesMatch.Score:P0}";
            var paramString = string.Join(", ",
                PropertiesMatch.MatchedPropertiesInOrder.Select(x => $"{x.PropertyInfo.PropertyType.Name} {x.PropertyInfo.Name}"));
            return start + (Constructor == null ? Method.Name : "ctor") + "(" + paramString + ")";
        }
    }
}