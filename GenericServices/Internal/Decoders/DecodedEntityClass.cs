// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using GenericLibsBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Remotion.Linq.Parsing;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedEntityClass
    {
        

        public Type EntityType { get; private set; }

        public ImmutableList<IKey> PrimaryKeys { get; private set; }

        public bool CanBeUpdatedByProperties { get; private set; }

        static IStatusGeneric<T> CreateFactory<T>( Type entityType, DbContext context) where T : class
        {
            var status = new StatusGenericHandler();
            var efType = context.Model.FindEntityType(entityType.FullName);
            if (efType == null)
            {
                status.AddError($"The class {entityType.Name} was not found in the {context.GetType().Name} DbContext.");
                return null;
            }

            var publicCtors = entityType.GetConstructors();
            var allPublicMethod = entityType.GetMethods();
            var publicStaticMethods = allPublicMethod.Where(x => x.IsStatic);
            var publicMethods = allPublicMethod.Where(x => x.IsStatic);
            var propertiesWithPublicSetter = entityType.GetProperties();

            throw new NotImplementedException();
        }
    }
}