// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Setup.Internal
{
    internal static class SetupEntitiesExtensions
    {
        public static void RegisterEntityClasses(this DbContext context)
        {
            foreach (var entityType in context.Model.GetEntityTypes()
                .Where(x => x.DefiningEntityType == null)) // this removes owned classes
            {
                context.RegisterDecodedEntityClass(entityType.ClrType);
            }
        }
    }
}