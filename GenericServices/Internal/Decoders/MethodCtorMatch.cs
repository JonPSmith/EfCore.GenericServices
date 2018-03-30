// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericServices.Configuration;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.Decoders
{
    internal enum HowTheyWereAskedFor { Unset, DefaultMatchToProperties, NamedMethodFromDtoClass, SpecifiedInPerDtoConfig}

    /// <summary>
    /// This is given a set of methods (with the same name) and picks the best matching method 
    /// that fits the set of possible properties that are available
    /// </summary>
    internal class MethodCtorMatch
    {
        public string Name { get; set; }
        public MethodInfo Method { get;}
        public ConstructorInfo Constructor { get; }

        public ParametersMatch PropertiesMatch { get;  }

        public HowTheyWereAskedFor HowDefined { get; }

        public bool IsParameterlessMethod => !PropertiesMatch.MatchedPropertiesInOrder.Any();

        public MethodCtorMatch(dynamic methodOrCtor, ParametersMatch propertiesMatch, HowTheyWereAskedFor howDefined)
        {
            Method = methodOrCtor as MethodInfo;
            if (Method != null)
                Name = Method.Name;
            Constructor = methodOrCtor as ConstructorInfo;
            if (Constructor != null)
                Name = DecodedNameTypes.Ctor.ToString();
            PropertiesMatch = propertiesMatch ?? throw new ArgumentNullException(nameof(propertiesMatch));
            HowDefined = howDefined;
        }

        public override string ToString()
        {
            var start = PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue
                ? "Match: "
                : $"Matched {PropertiesMatch.MatchedPropertiesInOrder.Count(x => x != null && x.Score >= PropertyMatch.NoMatchAtAll)}"+
                  $" params out of {PropertiesMatch.MatchedPropertiesInOrder.Count}. Score {PropertiesMatch.Score:P0} ";
            var paramString = string.Join(", ", PropertiesMatch.MatchedPropertiesInOrder.Select(x => x.ToString()));
            return start + Name + "(" + paramString + ")";
        }

        public string ToStringShort()
        {
            return $"{Name}({PropertiesMatch.MatchedPropertiesInOrder.Count} params)";
        }


    }
}