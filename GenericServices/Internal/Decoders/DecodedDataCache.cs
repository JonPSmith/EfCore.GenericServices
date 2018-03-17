// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using GenericLibsBase;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.Decoders
{
    internal static class DecodedDataCache
    {
        public static Type GetLinkInterfaceFromDto(this Type entityOrDto)
        {
            var linkInterface = entityOrDto.GetInterface(DecodedDto.NameILinkToEntity);
            return linkInterface?.GetGenericArguments().Single();
        }

        public static DecodedEntityClass GetUnderlyingEntityInfo(this DbContext context, Type entityOrDto)
        {
            if (EntityInfoCache.ContainsKey(entityOrDto)) return EntityInfoCache[entityOrDto];

            //If the entity type is found in the LinkToEntity interface it returns that, otherwise the called type because it must be the entity
            var entityType = entityOrDto.GetLinkInterfaceFromDto() ?? entityOrDto;
            return context.GetEntityClassInfo(entityType);
        }

        private static readonly ConcurrentDictionary<Type, DecodedDto> DecodedDtoCache = new ConcurrentDictionary<Type, DecodedDto>();

        public static IStatusGeneric<DecodedDto> GetDtoInfo(this Type classType, DecodedEntityClass entityInfo)
        {
            var status = new StatusGenericHandler<DecodedDto>();
            if (classType.IsPublic || classType.IsNestedPublic)
                status.Result = DecodedDtoCache.GetOrAdd(classType, type => new DecodedDto(classType, entityInfo));
            else
                status.AddError($"Sorry, but the DTO/ViewModel class '{classType.Name}' must be public for GenericServices to work.");

            return status;
        }

        private static readonly ConcurrentDictionary<Type, DecodedEntityClass> EntityInfoCache = new ConcurrentDictionary<Type, DecodedEntityClass>();

        private static DecodedEntityClass GetEntityClassInfo(this DbContext context, Type classType) 
        {
            return EntityInfoCache.GetOrAdd(classType, type => new DecodedEntityClass(classType, context));
        }

    }
}