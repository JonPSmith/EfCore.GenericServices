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
            var copierType = myGeneric.MakeGenericType(tDto, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(copierType, new object[] { context, mapper});
        }

        public class GenericCopier<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DbContext _context;
            private readonly IMapper _mapper;

            public GenericCopier(DbContext context, IMapper mapper)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }
                
            public void MapDtoToEntity(TDto dto, object entity)
            {
                var output = _mapper.Map(dto, entity);
            }

            public void LoadExistingAndMap(TDto dto, params object[] keys)
            {
                var entity = _context.Set<TEntity>().Find(keys);
                _mapper.Map(dto, entity);
            }
        }
    }
}