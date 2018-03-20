// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using GenericLibsBase;
using GenericServices.Configuration;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedDto : StatusGenericHandler
    {
        private readonly List<MethodInfo> _availableSetterMethods = new List<MethodInfo>();

        public Type DtoType { get; }
        public Type LinkedToType { get; }
        public ImmutableList<DecodedDtoProperty> PropertyInfos { get; }

        public ImmutableList<MethodInfo> AvailableSetterMethods => _availableSetterMethods.ToImmutableList();

        public DecodedDto(Type dtoType, DecodedEntityClass entityInfo, 
            IGenericServiceConfig overallConfig, PerDtoConfig perDtoConfig)
        {
            DtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            LinkedToType = entityInfo.EntityType;
            PropertyInfos = dtoType.GetProperties()
                .Select(x => new DecodedDtoProperty(x, 
                        BestPropertyMatch.FindMatch(x, entityInfo.PrimaryKeyProperties ).Score >= PropertyMatch.PerfectMatchValue))
                .ToImmutableList();

            if (entityInfo.CanBeUpdatedViaMethods || perDtoConfig?.UpdateMethods != null)
                _availableSetterMethods = MatchUpdateMethods(entityInfo, perDtoConfig?.UpdateMethods);
        }

        private List<MethodInfo> MatchUpdateMethods(DecodedEntityClass entityInfo, string updateMethods)
        {
            throw new NotImplementedException();
        }
    }
}