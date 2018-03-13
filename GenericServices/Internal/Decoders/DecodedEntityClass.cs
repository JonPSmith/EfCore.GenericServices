// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GenericLibsBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedEntityClass
    {

        public Type EntityType { get; private set; }

        public IReadOnlyList<IProperty> PrimaryKeyProperties { get; private set; }

        public DecodedClass EntityClassInfo { get; private set; }

        private DecodedEntityClass(Type entityType, IReadOnlyList<IProperty> primaryKeyProperties, DecodedClass entityClassInfo)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PrimaryKeyProperties = primaryKeyProperties;
            EntityClassInfo = entityClassInfo ?? throw new ArgumentNullException(nameof(entityClassInfo));
        }

        public static IStatusGeneric<DecodedEntityClass> CreateFactory( Type entityType, DbContext context)
        {
            var status = new StatusGenericHandler<DecodedEntityClass>();
            var efType = context.Model.FindEntityType(entityType.FullName);
            if (efType == null)
            {
                status.AddError($"The class {entityType.Name} was not found in the {context.GetType().Name} DbContext.");
                return status;
            }

            var primaryKeys = efType.GetKeys().Where(x => x.IsPrimaryKey()).ToList();
            if (primaryKeys.Count != 1)
            {
                status.AddError($"The class {entityType.Name} has {primaryKeys.Count} primary keys. I can't handle that.");
                return status;
            }

            status.Result = new DecodedEntityClass(entityType,
                primaryKeys.Single().Properties,
                new DecodedClass(entityType));
            return status;
        }
    }
}