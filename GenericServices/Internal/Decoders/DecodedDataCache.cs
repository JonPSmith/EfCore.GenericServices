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
        public static DecodedEntityClass GetUnderlyingEntityInfo(this Type entityOrDto, DbContext context)
        {
            if (EntityInfoCache.ContainsKey(entityOrDto)) return EntityInfoCache[entityOrDto];

            var linkInterface = entityOrDto.GetInterface(DecodedDto.NameILinkToEntity); ;
            var entityType = linkInterface == null
                ? entityOrDto //Its not a class marked with the ILinkToEntity<T> interface, so it must be an entity class
                : linkInterface.GetGenericArguments().Single();
            return entityType.GetEntityClassInfo(context);
        }

        private static readonly ConcurrentDictionary<Type, DecodedDto> DecodedDtoCache = new ConcurrentDictionary<Type, DecodedDto>();

        public static IStatusGeneric<DecodedDto> GetDtoInfo(this Type classType, DecodedEntityClass entityInfo)
        {
            var status = new StatusGenericHandler<DecodedDto>();
            if (!classType.IsPublic)
                status.AddError("Sorry, but the DTO/VM class must be public for GenericServices to work.");
            else
            {
                status.Result = DecodedDtoCache.GetOrAdd(classType, type => new DecodedDto(classType, entityInfo));
            }

            return status;
        }

        private static readonly ConcurrentDictionary<Type, DecodedEntityClass> EntityInfoCache = new ConcurrentDictionary<Type, DecodedEntityClass>();

        private static DecodedEntityClass GetEntityClassInfo(this Type classType, DbContext context) 
        {
            return EntityInfoCache.GetOrAdd(classType, type => new DecodedEntityClass(classType, context));
        }

    }
}