// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedDto
    {
        private readonly List<MethodCtorMatch> _matchedSetterMethods;
        private readonly List<MethodCtorMatch> _matchedCtorsAndStaticMethods;

        private List<MethodCtorMatch> _allPossibleSetterMatches;
        private List<MethodCtorMatch> _allPossibleCtorsAndStaticMatches;

        private readonly MethodCtorMatcher _matcher;
        private readonly PerDtoConfig _perDtoConfig; //can be null

        public Type DtoType { get; }
        public DecodedEntityClass LinkedEntityInfo { get; }
        public ImmutableList<DecodedDtoProperty> PropertyInfos { get; }


        /// <summary>
        /// This contains the different way the entity can be created
        /// </summary>
        public ImmutableList<MethodCtorMatch> MatchedCtorsAndStaticMethods =>
            _matchedCtorsAndStaticMethods.ToImmutableList();
        
        public DecodedDto(Type dtoType, DecodedEntityClass entityInfo,
            IGenericServicesConfig publicConfig, PerDtoConfig perDtoConfig)
        {
            DtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            _matcher = new MethodCtorMatcher(publicConfig.NameMatcher);
            _perDtoConfig = perDtoConfig; //can be null
            LinkedEntityInfo = entityInfo;
            if (entityInfo.EntityStyle == EntityStyles.OwnedType)
                throw new InvalidOperationException($"{DtoType.Name}: You cannot use ILinkToEntity<T> with an EF Core Owned Type.");
            if (entityInfo.EntityStyle == EntityStyles.HasNoKey)
                //If HasNoKey or is OwnedType then exit immediately as properties etc 
                return;

            PropertyInfos = dtoType.GetProperties()
                .Select(x => new DecodedDtoProperty(x, 
                        BestPropertyMatch.FindMatch(x, entityInfo.PrimaryKeyProperties ).Score >= PropertyMatch.PerfectMatchValue))
                .ToImmutableList();

            if(!PropertyInfos.Any())
                throw new InvalidOperationException($"The {DtoType.Name} class inherits ILinkToEntity<T> but has no properties in it!");

            if (entityInfo.CanBeUpdatedViaMethods)
                _matchedSetterMethods = PreloadPossibleMethodCtorMatches(MatchMethodsToProperties(entityInfo), 
                    new DecodeName(_perDtoConfig?.UpdateMethod), null);
            if (entityInfo.CanBeCreatedByCtorOrStaticMethod)
                _matchedCtorsAndStaticMethods = PreloadPossibleMethodCtorMatches(MatchCtorsAndStaticMethodsToProperties(entityInfo),
                    new DecodeName(_perDtoConfig?.UpdateMethod), null);
        }

        /// <summary>
        /// This returns true if the SaveChanges should be validated
        /// Done this way to allow unit tests to have different DtoAccessValidateOnSave settings
        /// </summary>
        /// <param name="publicConfig"></param>
        /// <returns></returns>
        public bool ShouldValidateOnSave(IGenericServicesConfig publicConfig)
        {
            return _perDtoConfig?.UseSaveChangesWithValidation ?? publicConfig.DtoAccessValidateOnSave;
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
                    throw new ArgumentException("You should only use Create or Update here", nameof(createOrUpdate));
            }
        }
        public MethodCtorMatch GetMethodToRun(DecodeName nameInfo, DecodedEntityClass entityInfo)
        {
            if (nameInfo.NameType == DecodedNameTypes.NoNameGiven)
            {
                var result = GetDefaultCtorOrMethod(_matchedSetterMethods, false);
                if (result != null)
                    return result;

                throw new InvalidOperationException($"The entity class {entityInfo.EntityType.Name} did find a method that matches the {DtoType.Name}." +
                                                    $" It only links methods where ALL the method's parameters can be fulfilled by the DTO/VM non-read-only properties." +
                                                    " The possible matches are \n" + string.Join("\n", _allPossibleSetterMatches));
            }

            return FindMethodCtorByName(nameInfo, _matchedSetterMethods, "method");
        }

        public MethodCtorMatch GetCtorStaticCreatorToRun(DecodeName nameInfo, DecodedEntityClass entityInfo)
        {
            if (nameInfo.NameType == DecodedNameTypes.NoNameGiven)
            {
                var result = GetDefaultCtorOrMethod(_matchedCtorsAndStaticMethods, true);
                if (result != null)
                    return result;

                throw new InvalidOperationException($"The entity class {entityInfo.EntityType.Name} did find an ctor/static method that matches the {DtoType.Name}." +
                                                    $" It only links ctor/static methods where ALL the method's parameters can be fulfilled by the DTO/VM non-read-only properties." +
                                                    " The possible matches are \n" + string.Join("\n", _allPossibleCtorsAndStaticMatches));
            }

            return FindMethodCtorByName(nameInfo, _matchedCtorsAndStaticMethods, "ctor/static methods");
        }

        //---------------------------------------------------------------------------------

        private static MethodCtorMatch FindMethodCtorByName(DecodeName nameInfo, List<MethodCtorMatch>listToScan, string errorString)
        {
            //IEnumerable<>
            var namedMethods = nameInfo.NameType == DecodedNameTypes.Ctor
                ? listToScan.Where(x => x.Constructor != null)
                : listToScan.Where(x => x.Name == nameInfo.Name);

            if (nameInfo.NumParams > 0)
                namedMethods = namedMethods.Where(x =>
                    x.PropertiesMatch.MatchedPropertiesInOrder.Count == nameInfo.NumParams);
            var result = namedMethods.ToList();
            var nameString = nameInfo.Name + (nameInfo.NumParams < 0 ? "" : $"({nameInfo.NumParams})");
            if (!result.Any() && errorString != null)
                throw new InvalidOperationException($"Could not find a {errorString} of name {nameString}. The {errorString} that fit the properties in the DTO/VM are:\n" +
                                string.Join("\n", listToScan.Select(x => x.ToString())));
            if (result.Count > 1)
            {
                if (errorString != null)
                    throw new InvalidOperationException($"There were multiple {errorString}s that fitted the name {nameString}. The possible options are:\n" +
                                                    string.Join("\n", result.Select(x => x.ToString())));
                return null;
            }
            return result.SingleOrDefault();
        }

        private MethodCtorMatch GetDefaultCtorOrMethod(List<MethodCtorMatch> listToScan, bool lookForCtors)
        {
            string errorString = lookForCtors ? "ctor/static method" : "method";
            //we group by the number of parameters and take the one with the longest parameter match
            var groupedByParams = (from ctorMethod in listToScan.Where(x => !x.IsParameterlessMethod)
                let numParams = ctorMethod.Method == null
                    ? ctorMethod.Constructor.GetParameters().Length
                    : ctorMethod.Method.GetParameters().Length
                group ctorMethod by numParams).ToList();
            if (!groupedByParams.Any())
            {
                var possibleOptions = lookForCtors ? _allPossibleCtorsAndStaticMatches : _allPossibleSetterMatches;
                throw new InvalidOperationException($"Could not find a {errorString} that matches the DTO. The {errorString} that fit the properties in the DTO/VM are:\n" +
                                                    string.Join("\n", possibleOptions.Select(x => x.ToString())));
            }

            var methodsWithParams = groupedByParams.OrderByDescending(x => x.Key).First().ToList();
            //var methodsWithParams = listToScan.Where(x => !x.IsParameterlessMethod)
            //    .GroupBy(x => x.Method.GetParameters().Length).OrderByDescending(x => x.Key).First().ToList();
            if (methodsWithParams.Count == 1)
                return methodsWithParams.Single();

            if (methodsWithParams.Any())
                throw new InvalidOperationException($"There are multiple {errorString}, so you need to define which one you want used via the {errorString} parameter. " +
                                                    "The possible options are:\n" +
                                                    string.Join("\n", listToScan.Select(x => x.ToString())));
            return null;
        }

        private List<MethodCtorMatch> PreloadPossibleMethodCtorMatches(List<MethodCtorMatch> allPossibleMatches, DecodeName nameInfo, string errorString)
        {
            if (nameInfo.NameType == DecodedNameTypes.AutoMapper)
                return new List<MethodCtorMatch>();
            if (nameInfo.NameType != DecodedNameTypes.NoNameGiven)
            {
                //name as been defined in perDtoConfig
                return new List<MethodCtorMatch>{FindMethodCtorByName(nameInfo,
                    allPossibleMatches, (errorString == null ? null : errorString + "(PerDtoConfig)"))};
            }
            //Nothing defined so try via the DTO name
            var nameFromDto = ExtractPossibleMethodNameFromDtoTypeName();
            var dtoNamed = new DecodeName(nameFromDto);
            var foundMatch = FindMethodCtorByName(dtoNamed, allPossibleMatches, null);
            if (foundMatch != null)
                return new List<MethodCtorMatch>{foundMatch};
            //otherwise return all the possible versions
            return allPossibleMatches;
        }

        private List<MethodCtorMatch> MatchMethodsToProperties(DecodedEntityClass entityInfo)
        {
            var nonReadOnlyPropertyInfo = PropertyInfos.Where(y => y.PropertyType != DtoPropertyTypes.ReadOnly)
                .Select(x => x.PropertyInfo).ToList();

            _allPossibleSetterMatches = _matcher.GradeAllMethods(entityInfo.PublicSetterMethods,
                nonReadOnlyPropertyInfo, HowTheyWereAskedFor.DefaultMatchToProperties).ToList();
            return _allPossibleSetterMatches.Where(x => x.PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue).ToList();
        }

        private List<MethodCtorMatch> MatchCtorsAndStaticMethodsToProperties(DecodedEntityClass entityInfo)
        {
            var nonReadOnlyPropertyInfo = PropertyInfos.Where(y => y.PropertyType != DtoPropertyTypes.ReadOnly)
                .Select(x => x.PropertyInfo).ToList();

            _allPossibleCtorsAndStaticMatches = _matcher.GradeAllCtorsAndStaticMethods(
                entityInfo.PublicStaticCreatorMethods, entityInfo.PublicCtors,
                nonReadOnlyPropertyInfo, HowTheyWereAskedFor.DefaultMatchToProperties).ToList();
            return _allPossibleCtorsAndStaticMatches.Where(x => x.PropertiesMatch.Score >= PropertyMatch.PerfectMatchValue).ToList();
        }

        private readonly string[] _endingsToRemove = new[] { "Dto", "VM", "ViewModel" };

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