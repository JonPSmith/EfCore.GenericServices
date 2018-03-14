// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class CreateCopier
    {
        public dynamic Accessor { get;  }

        public CreateCopier(DbContext context, IMapper mapper, Type tDto, DecodedEntityClass entityInfo)
        {
            var myGeneric = typeof(GenericCopier<,>);
            var projectorType = myGeneric.MakeGenericType(tDto, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(projectorType, new object[] { context, mapper, entityInfo });
        }

        public class GenericCopier<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DbContext _context;
            private readonly IMapper _mapper;
            private readonly DecodedEntityClass _entityInfo;

            public GenericCopier(DbContext context, IMapper mapper, DecodedEntityClass entityInfo)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            }
                
            public void MapDtoToEntity(TDto dto, object entity)
            {
                _mapper.Map(dto, entity);
            }

            public void LoadExistingAndMap(TDto dto, params object[] keys)
            {
                var entity = _context.Set<TEntity>().Find(keys);
                _mapper.Map(dto, entity);
            }
        }
    }
}