// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Startup.Internal
{
    internal static class SetupEntitiesInDbContext
    {
        public static IStatusGeneric RegisterEntityClasses(this DbContext context)
        {
            var status = new StatusGenericHandler();
            var entityNameList = new List<string>();
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var entityInfo = context.RegisterDecodedEntityClass(entityType.ClrType);
                entityNameList.Add(entityInfo.EntityType.Name);
            }

            status.Message = "Entity types: " + string.Join(", ", entityNameList);
            return status;
        }
    }
}