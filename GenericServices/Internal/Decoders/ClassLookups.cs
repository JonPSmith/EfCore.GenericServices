// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.Decoders
{
    internal static class ClassLookups
    {
        public static DecodedEntityClass GetUnderlyingEntityInfo(this Type entityOrDto, DbContext context)
        {
            var entityTypeFromLinkInterface = entityOrDto.GetInterface("ILinkToEntity`1");
            var entityType = entityTypeFromLinkInterface == null
                ? entityOrDto //Its not a class marjed with the ILinkToEntity<T> interface, so it must be an entity class
                : entityTypeFromLinkInterface.GetGenericArguments().Single();
            return entityType.GetEntityClassInfo(context);
        }


    }
}