// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericServices.Configuration;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.Decoders
{
    internal class MethodCtorMatcher
    {
        private readonly MatchNameAndType _publicPropMatcher;

        public MethodCtorMatcher(MatchNameAndType publicPropMatcher)
        {
            _publicPropMatcher = publicPropMatcher;
        }

        /// <summary>
        /// This takes a set of methods and grades them as to whether they can be called by the parameters in the DTO.
        /// This allows extra parameters to be in the DTO for other purposes, and still fit 
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="propertiesToCheck"></param>
        /// <param name="howDefined"></param>
        /// <returns>It returns a collection of MethodCtorMatch, with the best scores first, with secondary sort order with longest number of params first</returns>
        public IEnumerable<MethodCtorMatch> GradeAllMethods(MethodInfo[] methods,
            List<PropertyInfo> propertiesToCheck, HowTheyWereAskedFor howDefined)
        {
            var result = methods.Select(method => new MethodCtorMatch(method,
                new ParametersMatch(method.GetParameters(), propertiesToCheck, InternalPropertyMatch), howDefined));

            return result.OrderByDescending(x => x.PropertiesMatch.MatchedPropertiesInOrder.Count);
        }

        /// <summary>
        /// Same as GradeAllMethods, but for ctors and static methods
        /// </summary>
        /// <param name="staticCreateMethods"></param>
        /// <param name="publicCtors"></param>
        /// <param name="propertiesToCheck"></param>
        /// <param name="howDefined"></param>
        /// <returns></returns>
        public IEnumerable<MethodCtorMatch> GradeAllCtorsAndStaticMethods(MethodInfo[] staticCreateMethods,
            ConstructorInfo[] publicCtors, List<PropertyInfo> propertiesToCheck,
            HowTheyWereAskedFor howDefined)
        {
            var result = staticCreateMethods.Select(method => new MethodCtorMatch(method,
                new ParametersMatch(method.GetParameters(), propertiesToCheck, InternalPropertyMatch), howDefined)).ToList();
            result.AddRange(publicCtors.Select(method => new MethodCtorMatch(method,
                new ParametersMatch(method.GetParameters(), propertiesToCheck, InternalPropertyMatch), howDefined)));

            return result.OrderByDescending(x => x.PropertiesMatch.MatchedPropertiesInOrder.Count);
        }

        /// <summary>
        /// This handles method properties that require injection of DbContext 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private PropertyMatch InternalPropertyMatch(string name, Type type, PropertyInfo propertyInfo)
        {
            if (type == typeof(DbContext) || type.IsSubclassOf(typeof(DbContext)))
                return new PropertyMatch(true, PropertyMatch.TypeMatchLevels.Match, null, MatchSources.DbContext, type);

            return _publicPropMatcher(name, type, propertyInfo);
        }
    }
}