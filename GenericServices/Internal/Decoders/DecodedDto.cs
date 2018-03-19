// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using GenericServices.Configuration;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedDto
    {
        public Type DtoType { get; }
        public Type LinkedToType { get; }
        public ImmutableList<DecodedDtoProperty> PropertyInfos { get; }

        public DecodedDto(Type dtoType, DecodedEntityClass entityInfo, 
            IGenericServiceConfig overallConfig, PerDtoConfig perDtoConfig)
        {
            DtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            LinkedToType = entityInfo.EntityType;
            PropertyInfos = dtoType.GetProperties()
                .Select(x => new DecodedDtoProperty(x, 
                        BestPropertyMatch.FindMatch(x, entityInfo.PrimaryKeyProperties ).Score > BestMethodCtorMatch.perfectMatchValue))
                .ToImmutableList();
        }
    }
}