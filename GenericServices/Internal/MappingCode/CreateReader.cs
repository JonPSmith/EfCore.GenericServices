// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using AutoMapper;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class CreateReader
    {
        public dynamic Accessor { get;  }

        public CreateReader(DbContext context, MapperConfiguration mapper, Type tDto, DecodedEntityClass entityInfo)
        {
            var myGeneric = typeof(GenericCopier<,>);
            var copierType = myGeneric.MakeGenericType(tDto, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(copierType, new object[] { context, mapper});
        }

        public class GenericCopier<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DbContext _context;
            private readonly MapperConfiguration _mapperConfig;

            public GenericCopier(DbContext context, MapperConfiguration mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapperConfig = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }
                
            public void MapDtoToEntity(TDto dto, object entity)
            {
                var output = _mapperConfig.CreateMapper().Map(dto, entity);
            }

            public TEntity ReturnExistingEntity(params object[] keys)
            {
                return _context.Set<TEntity>().Find(keys);
            }
        }
    }
}