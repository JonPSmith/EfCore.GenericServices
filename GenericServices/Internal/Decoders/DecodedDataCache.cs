// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.Decoders
{
    internal static class DecodedDataCache
    {
        private static readonly ConcurrentDictionary<Type, ImmutableList<PropertyInfo>> KeyCache = new ConcurrentDictionary<Type, ImmutableList<PropertyInfo>>();

        /// <summary>
        /// This is used to find the properties in the DTO
        /// </summary>
        /// <returns></returns>
        public static ImmutableList<PropertyInfo> GetPublicProperties(this Type classType)
        {
            return KeyCache.GetOrAdd(classType, type => classType.GetProperties().ToImmutableList());
        }

        private static readonly ConcurrentDictionary<Type, DecodedEntityClass> EntityInfoCache = new ConcurrentDictionary<Type, DecodedEntityClass>();

        public static DecodedEntityClass GetEntityClassInfo(this Type classType, DbContext context) 
        {
            return EntityInfoCache.GetOrAdd(classType, type => new DecodedEntityClass(classType, context));
        }


    }
}