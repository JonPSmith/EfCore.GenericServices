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
using Remotion.Linq.Parsing;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedEntityClass
    {

        public Type EntityType { get; private set; }

        public ImmutableList<IKey> PrimaryKeys { get; private set; }

        public DecodedClass EntityClassInfo { get; private set; }

        private DecodedEntityClass(Type entityType, IEnumerable<IKey> primaryKeys, DecodedClass entityClassInfo)
        {
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            PrimaryKeys = primaryKeys?.ToImmutableList() ?? throw new ArgumentNullException(nameof(primaryKeys));
            EntityClassInfo = entityClassInfo ?? throw new ArgumentNullException(nameof(entityClassInfo));
        }

        static IStatusGeneric<DecodedEntityClass> CreateFactory( Type entityType, DbContext context)
        {
            var status = new StatusGenericHandler<DecodedEntityClass>();
            var efType = context.Model.FindEntityType(entityType.FullName);
            if (efType == null)
            {
                status.AddError($"The class {entityType.Name} was not found in the {context.GetType().Name} DbContext.");
                return status;
            }

            status.Result = new DecodedEntityClass(entityType, 
                efType.GetKeys().Where(x => x.IsPrimaryKey()),
                new DecodedClass(entityType));
            return status;
        }
    }
}