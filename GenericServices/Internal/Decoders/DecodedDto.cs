// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedDto
    {

        public Type DtoType { get; }
        public Type LinkedToType { get; }
        public ImmutableList<DecodedDtoProperty> PropertyInfos { get; }

        public DecodedDto(Type dtoType, DecodedEntityClass entityInfo)
        {
            DtoType = dtoType ?? throw new ArgumentNullException(nameof(dtoType));
            var linkInterface = dtoType.GetInterface(DecodedDtoExtensions.InterfaceNameILinkToEntity);
            if (linkInterface == null)
                throw new InvalidOperationException($"Class {dtoType.Name} isn't an entity class, so it should have the {DecodedDtoExtensions.HumanReadableILinkToEntity}<> interface.");
            LinkedToType = linkInterface.GetGenericArguments().Single();
            PropertyInfos = dtoType.GetProperties()
                .Select(x => new DecodedDtoProperty(x, 
                        BestPropertyMatch.FindMatch(x, entityInfo.PrimaryKeyProperties ).Score > BestMethodCtorMatch.perfectMatchValue))
                .ToImmutableList();
        }
    }
}