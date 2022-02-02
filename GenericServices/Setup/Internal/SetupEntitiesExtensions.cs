// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Setup.Internal
{
    internal static class SetupEntitiesExtensions
    {
        public static void RegisterEntityClasses(this DbContext context)
        {
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                context.RegisterDecodedEntityClass(entityType.ClrType);
            }
        }
    }
}