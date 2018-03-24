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
        private readonly string[] _endingsToRemove = new[] { "Dto", "VM", "ViewModel" };
        private readonly List<MethodCtorMatch> _matchedSetterMethods;
        private readonly List<MethodCtorMatch> _matchedCtorsAndStaticMethods;

        private readonly IExpandedGlobalConfig _overallConfig;
        private readonly PerDtoConfig _perDtoConfig;

        public Type DtoType { get; }
        public Type LinkedToType { get; }
        public ImmutableList<DecodedDtoProperty> PropertyInfos { get; }
        /// <summary>
        /// This is true if the the validating SaveChanges extention method should be called 
        /// </summary>
        public bool ValidateOnSave { get; }

        /// <summary>
        /// This contains the different way the entity can be created
        /// </summary>
        public ImmutableList<MethodCtorMatch> MatchedCtorsAndStaticMethods =>
            _matchedCtorsAndStaticMethods.ToImmutableList();
        
        public DecodedDto(Type dtoType, DecodedEntityClass entityInfo,
            IExpandedGlobalConfig overallConfig, PerDtoConfig perDtoConfig)
        {
            DtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            _overallConfig = overallConfig ?? throw new ArgumentNullException(nameof(overallConfig));
            _perDtoConfig = perDtoConfig; //can be null
            LinkedToType = entityInfo.EntityType;

            ValidateOnSave = _overallConfig.PublicConfig.CrudSaveUseValidation;
            if (_perDtoConfig?.UseSaveChangesWithValidation != null)
                ValidateOnSave = (bool) _perDtoConfig?.UseSaveChangesWithValidation;

            PropertyInfos = dtoType.GetProperties()
                .Select(x => new DecodedDtoProperty(x, 
                        BestPropertyMatch.FindMatch(x, entityInfo.PrimaryKeyProperties ).Score >= PropertyMatch.PerfectMatchValue))
                .ToImmutableList();

            if (entityInfo.CanBeUpdatedViaMethods)
                _matchedSetterMethods = PreloadPossibleMethodCtorMatches(MatchMethodsToProperties(entityInfo), 
                    new DecodeName(_perDtoConfig?.UpdateMethod), null);
            if (entityInfo.CanBeCreatedByCtorOrStaticMethod)
                _matchedCtorsAndStaticMethods = PreloadPossibleMethodCtorMatches(MatchCtorsAndStaticMethodsToProperties(entityInfo),
                    new DecodeName(_perDtoConfig?.UpdateMethod), null);
        }

        /// <summary>
        /// This checks if any name was given in the command, or the perDtoConfig specifies a name
        /// </summary>
        /// <param name="nameGivenInCommand"></param>
        /// <param name="createOrUpdate"></param>
        /// <returns></returns>
        public DecodeName GetSpecifiedName(string nameGivenInCommand, CrudTypes createOrUpdate)
        {
            var result = new DecodeName(nameGivenInCommand);
            if (result.NameType != DecodedNameTypes.NoNameGiven)
                return result;

            switch (createOrUpdate)
            {
                case CrudTypes.Create:
                    return new DecodeName(_perDtoConfig?.CreateMethod);
                case CrudTypes.Update:
                    return new DecodeName(_perDtoConfig?.UpdateMethod);
                default:
                    throw new ArgumentException("You should only use Create ot Update here", nameof(createOrUpdate));
            }
        }
        public MethodCtorMatch GetMethodToRun(DecodeName nameInfo, DecodedEntityClass entityInfo)
        {
            if (nameInfo.NameType == DecodedNameTypes.NoNameGiven)
                return GetDefaultSetterMethod(entityInfo, _matchedSetterMethods, "method");

            return FindMethodCtorByName(nameInfo.Name, nameInfo.NumParams, _matchedSetterMethods, "method");
        }

        public MethodCtorMatch GetCtorStaticFactoryToRun(DecodeName nameInfo, DecodedEntityClass entityInfo)
        {
            if (nameInfo.NameType == DecodedNameTypes.NoNameGiven)
                return GetDefaultSetterMethod(entityInfo, _matchedCtorsAndStaticMethods, "ctor/static method");

            return FindMethodCtorByName(nameInfo.Name, nameInfo.NumParams, _matchedCtorsAndStaticMethods, "ctor/static method");
        }

        //---------------------------------------------------------------------------------

        private static MethodCtorMatch FindMethodCtorByName(string name, int numParams, List<MethodCtorMatch>listToScan, string errorString)
        {
            var namedMethods = listToScan.Where(x => x.Name == name);
            if (numParams > 0)
                namedMethods = namedMethods.Where(x =>
                    x.PropertiesMatch.MatchedPropertiesInOrder.Count == numParams);
            var result = namedMethods.ToList();
            var nameString = name + (numParams < 0 ? "" : $"({numParams})");
            if (!result.Any() && errorString != null)
                throw new InvalidOperationException($"Could not find a {errorString} of name {nameString}. The {errorString} that fit the properties in the DTO/VM are:\n" +
                                string.Join("\n", listToScan.Select(x => x.ToStringShort())));
            if (result.Count > 1)
            {
                if (errorString != null)
                    throw new InvalidOperationException($"There were multiple {errorString}s that fitted the name {nameString}. The possible options are:\n" +
                                                    string.Join("\n", result.Select(x => x.ToStringShort())));
                return null;
            }
            return result.SingleOrDefault();
        }

        private static MethodCtorMatch GetDefaultSetterMethod(DecodedEntityClass entityInfo, List<MethodCtorMatch> listToScan, string errorString)
        {
            var methodsWithParams = listToScan.Where(x => !x.IsParameterlessMethod).ToList();
            if (methodsWithParams.Count == 1)
                return methodsWithParams.Single();

            if (methodsWithParams.Any())
                throw new InvalidOperationException($"There are multiple {errorString}, so you need to define which one you want used via the {errorString} parameter. "+
                                                    "The possible options are:\n" +
                                                    string.Join("\n", listToScan.Select(x => x.ToStringShort())));
            throw new InvalidOperationException($"The entity class {entityInfo.EntityType.GetNameForClass()} did not have an {errorString} linked to this DTO/VM."+
                                                $" It only links {errorString} where all the method's parameters can be forfilled by the DTO/VM non-read-only properties.");
        }

        private List<MethodCtorMatch> PreloadPossibleMethodCtorMatches(List<MethodCtorMatch> allPossibleMatches, DecodeName nameInfo, string errorString)
        {
            if (nameInfo.NameType == DecodedNameTypes.AutoMapper)
                return new List<MethodCtorMatch>();
            if (nameInfo.NameType != DecodedNameTypes.NoNameGiven)
            {
                //name as been defined in perDtoConfig
                return new List<MethodCtorMatch>{FindMethodCtorByName(nameInfo.Name, nameInfo.NumParams,
                    allPossibleMatches, (errorString == null ? null : errorString + "(PerDtoConfig)"))};
            }
            //Nothing defined so try via the DTO name
            var nameFromDto = ExtractPossibleMethodNameFromDtoTypeName();
            var foundMatch = FindMethodCtorByName(nameFromDto, -1, allPossibleMatches, null);
            if (foundMatch != null)
                return new List<MethodCtorMatch>{foundMatch};
            //otherwise return all the possible versions
            return allPossibleMatches;
        }

        private List<MethodCtorMatch> MatchMethodsToProperties(DecodedEntityClass entityInfo)
        {
            var nonReadOnlyPropertyInfo = PropertyInfos.Where(y => y.PropertyType != DtoPropertyTypes.ReadOnly)
                .Select(x => x.PropertyInfo).ToList();

            var matches = MethodCtorMatch.GradeAllMethods(entityInfo.PublicSetterMethods,
                nonReadOnlyPropertyInfo, HowTheyWereAskedFor.DefaultMatchToProperties,
                _overallConfig.InternalPropertyMatch);
            return matches.Where(x => x.PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue).ToList();
        }

        private List<MethodCtorMatch> MatchCtorsAndStaticMethodsToProperties(DecodedEntityClass entityInfo)
        {
            var nonReadOnlyPropertyInfo = PropertyInfos.Where(y => y.PropertyType != DtoPropertyTypes.ReadOnly)
                .Select(x => x.PropertyInfo).ToList();

            var result = new List<MethodCtorMatch>();
            var matches = MethodCtorMatch.GradeAllCtorsAndStaticMethods(entityInfo.PublicStaticFactoryMethods, entityInfo.PublicCtors,
                nonReadOnlyPropertyInfo, HowTheyWereAskedFor.DefaultMatchToProperties,
                _overallConfig.InternalPropertyMatch);
            return matches.Where(x => x.PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue).ToList();
        }

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