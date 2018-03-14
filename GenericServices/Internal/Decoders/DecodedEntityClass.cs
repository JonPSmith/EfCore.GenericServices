// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using GenericLibsBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedEntityClass
    {
        public Type EntityType { get; private set; }

        public ImmutableList<PropertyInfo> PrimaryKeyProperties { get; private set; }

        public DecodedTargetClass EntityClassInfo { get; private set; }

        public DecodedEntityClass(Type entityType, DbContext context)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            var efType = context.Model.FindEntityType(entityType.FullName);
            if (efType == null)
            {
                throw new InvalidOperationException($"The class {entityType.Name} was not found in the {context.GetType().Name} DbContext."+
                                                    " The class must be either be an entity class derived from the GenericServiceDto/Async class.");
            }

            var primaryKeys = efType.GetKeys().Where(x => x.IsPrimaryKey()).ToImmutableList();
            if (primaryKeys.Count != 1)
            {
                throw new InvalidOperationException($"The class {entityType.Name} has {primaryKeys.Count} primary keys. I can't handle that.");
            }

            PrimaryKeyProperties = primaryKeys.Single().Properties.Select(x => x.PropertyInfo).ToImmutableList();
            EntityClassInfo = new DecodedTargetClass(entityType);
        }
    }
}