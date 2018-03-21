// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedDto : StatusGenericHandler
    {
        private readonly IExpandedGlobalConfig _overallConfig;

        public Type DtoType { get; }
        public Type LinkedToType { get; }
        public ImmutableList<DecodedDtoProperty> PropertyInfos { get; }

        /// <summary>
        /// This contains the update methods that are available to the user - if there is more than one the user must define which one they want
        /// </summary>
        public ImmutableList<MethodCtorMatch> MatchedSetterMethods { get; }

        public DecodedDto(Type dtoType, DecodedEntityClass entityInfo,
            IExpandedGlobalConfig overallConfig, PerDtoConfig perDtoConfig)
        {
            DtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            _overallConfig = overallConfig ?? throw new ArgumentNullException(nameof(overallConfig));
            LinkedToType = entityInfo.EntityType;
            PropertyInfos = dtoType.GetProperties()
                .Select(x => new DecodedDtoProperty(x, 
                        BestPropertyMatch.FindMatch(x, entityInfo.PrimaryKeyProperties ).Score >= PropertyMatch.PerfectMatchValue))
                .ToImmutableList();

            if (entityInfo.CanBeUpdatedViaMethods || perDtoConfig?.UpdateMethods != null)
                MatchedSetterMethods = MatchUpdateMethods(entityInfo, perDtoConfig?.UpdateMethods).ToImmutableList();
        }

        public MethodCtorMatch FindSetterMethod(DecodeName nameInfo)
        {
            var namedMethods = MatchedSetterMethods.Where(x => x.Method.Name == nameInfo.Name);
            if (nameInfo.NumParams > 0)
                namedMethods = namedMethods.Where(x =>
                    x.PropertiesMatch.MatchedPropertiesInOrder.Count == nameInfo.NumParams);
            var result = namedMethods.ToList();
            if (!result.Any())
                throw new InvalidOperationException($"Could not find a method of name {nameInfo}. The possible options are:\n" +
                                string.Join("\n", MatchedSetterMethods.Select(x => x.ToStringShort())));
            if (result.Count > 1)
                throw new InvalidOperationException($"There were multiple methods that fitted the name {nameInfo}. The possible options are:\n" +
                                string.Join("\n", result.Select(x => x.ToStringShort())));
            return result.Single();
        }

        public MethodCtorMatch GetDefaultSetterMethod(DecodedEntityClass entityInfo)
        {
            var methodsWithParams = MatchedSetterMethods.Where(x => !x.IsParameterlessMethod).ToList();
            if (methodsWithParams.Count == 1)
                return methodsWithParams.Single();

            if (methodsWithParams.Any())
                throw new InvalidOperationException($"There are multiple methods, so you need to define which one you want used via the methodName parameter. "+
                                                    "The possible options are:\n" +
                                                    string.Join("\n", MatchedSetterMethods.Select(x => x.ToStringShort())));
            throw new InvalidOperationException($"The entity class {entityInfo.EntityType.GetNameForClass()} did not have an methods linked to this DTO/VM."+
                                                " It only links methods where all the method's parameters can be forfilled by the DTO/VM non-read-only properties.");
        }

        //------------------------------------------------------------------
        //private methods

        private List<MethodCtorMatch> MatchUpdateMethods(DecodedEntityClass entityInfo, string updateMethods)
        {
            var nonReadOnlyPropertyInfo = PropertyInfos.Where(y => y.PropertyType != DtoPropertyTypes.ReadOnly)
                .Select(x => x.PropertyInfo).ToList();

            var result = new List<MethodCtorMatch>();
            if (updateMethods != null)
            {          
                //The user has defined the exact update methods they want matched
                foreach (var methodName in updateMethods.Split(',').Select(x => x.Trim()))
                {
                    var matches = MethodCtorMatch.GradeAllMethods(FindMethodsWithGivenName(entityInfo, methodName, true).ToArray(),
                         nonReadOnlyPropertyInfo, HowTheyWereAskedFor.SpecifiedInPerDtoConfig, _overallConfig.InternalPropertyMatch);
                    var firstMatch = matches.FirstOrDefault();
                    if (firstMatch == null || firstMatch.PropertiesMatch.Score < PropertyMatch.PerfectMatchValue)
                        AddError(
                            $"You asked for update method {methodName}, but could not find a exact match of parameters." +
                            (firstMatch == null ? "" : $" Closest fit is {firstMatch}."));
                    else
                    {
                        result.Add(firstMatch);
                    }
                }
            }
            else
            {
                //The developer hasn't defined want methods should be mapped, so we take a guess based on the DTO's name
                var methodNameToLookFor = ExtractPossibleMethodNameFromDtoTypeName();
                var methodsThatMatchedDtoName = FindMethodsWithGivenName(entityInfo, methodNameToLookFor, false);
                if (methodsThatMatchedDtoName.Any())
                {
                    var matches = MethodCtorMatch.GradeAllMethods(methodsThatMatchedDtoName.ToArray(),
                        nonReadOnlyPropertyInfo, HowTheyWereAskedFor.NamedMethodFromDtoClass, _overallConfig.InternalPropertyMatch);
                    var firstMatch = matches.FirstOrDefault();
                    if (firstMatch != null || firstMatch.PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue)
                        result.Add(firstMatch);
                }
                if (!result.Any())
                {
                    //Nothing else has worked so do a default scan of all methods
                    var matches = MethodCtorMatch.GradeAllMethods(entityInfo.PublicSetterMethods.ToArray(),
                        nonReadOnlyPropertyInfo, HowTheyWereAskedFor.DefaultMatchToProperties, _overallConfig.InternalPropertyMatch);
                    result.AddRange(matches.Where(x => x.PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue));
                }
            }

            return result;
        }

        private List<MethodInfo> FindMethodsWithGivenName(DecodedEntityClass entityInfo, string methodName, bool raiseErrorIfNotThere)
        {
            var foundMethods = new List<MethodInfo>();
            var methodsToMatch = entityInfo.PublicSetterMethods
                .Where(x => x.Name == methodName).ToList();
            if (!methodsToMatch.Any() && raiseErrorIfNotThere)
            {
                AddError(
                    $"In the PerDtoConfig you asked for the method {methodName}," +
                    $" but that wasn't found in entity class {entityInfo.EntityType.GetNameForClass()}.");
            }
            else
            {
                foundMethods.AddRange(methodsToMatch);
            }

            return foundMethods;
        }

        private readonly string[] _endingsToRemove = new[] {"Dto", "VM", "ViewModel"};

        private string ExtractPossibleMethodNameFromDtoTypeName()
        {
            var name = DtoType.Name;
            foreach (var ending in _endingsToRemove)
            {
                if (name.EndsWith(ending, StringComparison.InvariantCultureIgnoreCase))
                    return name.Substring(0, name.Length - ending.Length);
            }

            return name;
        }
    }
}