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
    internal class BestMethodCtorMatch
    {
        public MethodInfo Method { get;  set; }
        public ConstructorInfo Constructor { get; }

        public ImmutableList<BestPropertyMatch> DtoPropertiesInOrder { get; set; }

        /// <summary>
        /// A Score of 1 means a perfect match. 
        /// </summary>
        public double Score { get; set; }

        private BestMethodCtorMatch(object methodOrCtor, List<BestPropertyMatch> propertyMatches)
        {
            Method = methodOrCtor as MethodInfo;
            Constructor = methodOrCtor as ConstructorInfo;
            DtoPropertiesInOrder = propertyMatches.ToImmutableList();
            Score = propertyMatches.Average(x => x.Score);
        }

        public static BestMethodCtorMatch FindMatch(ImmutableList<PropertyInfo> propertyInfos, dynamic[] whattoConsider)
        {
            if (whattoConsider.Length == 0)
                return null;

            BestMethodCtorMatch bestSoFar = null;
            foreach (var method in whattoConsider.Where(x => x.GetParameters().Length > 0).OrderByDescending(x => x.GetParameters().Length))
            {
                var matchedProps = new List<BestPropertyMatch>();
                foreach (var methodParam in method.GetParameters())
                {
                    matchedProps.Add(BestPropertyMatch.FindMatch(methodParam.Name, methodParam.ParameterType, propertyInfos));
                }

                var match = new BestMethodCtorMatch(method, matchedProps);
                if (bestSoFar == null || bestSoFar.Score < match.Score)
                    bestSoFar = match;
            }

            return bestSoFar;
        }

        public override string ToString()
        {
            var start = Score >= PropertyMatch.PerfectMatchValue
                ? "Match: "
                : $"Closest match at {Score:P0}: ";
            var paramString = string.Join(", ",
                DtoPropertiesInOrder.Select(x => $"{x.PropertyInfo.PropertyType.Name} {x.PropertyInfo.Name}"));
            return start + (Constructor == null ? Method.Name : "ctor") + "(" + paramString + ")";
        }
    }
}