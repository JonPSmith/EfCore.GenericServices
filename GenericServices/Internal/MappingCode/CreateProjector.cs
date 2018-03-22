// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.LinqBuilders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class CreateProjector
    {
        public dynamic Accessor { get;  }

        public CreateProjector(DbContext context, MapperConfiguration mapper, Type tOut, DecodedEntityClass entityInfo)
        {
            var myGeneric = typeof(GenericProjector<,>);
            var projectorType = myGeneric.MakeGenericType(tOut, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(projectorType, new object[] { context, mapper, entityInfo});
        }

        public class GenericProjector<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DbContext _context;
            private readonly MapperConfiguration _mapperConfig;
            private readonly DecodedEntityClass _entityInfo;

            public GenericProjector(DbContext context, MapperConfiguration mapper, DecodedEntityClass entityInfo)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _mapperConfig = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            }

            public IQueryable<TDto> GetViaKeysWithProject(params object[] keys)
            {
                var predicate = _entityInfo.PrimaryKeyProperties.CreateFilter<TEntity>(keys);
                return _context.Set<TEntity>().Where(predicate).ProjectTo<TDto>(_mapperConfig);
            }

            public IQueryable<TDto> ProjectAndThenApplyWhereExpression(Expression<Func<TDto, bool>> whereExpression)
            {
                return _context.Set<TEntity>().ProjectTo<TDto>(_mapperConfig).Where(whereExpression);
            }

            public IQueryable<TDto> GetManyProjectedNoTracking()
            {
                return _context.Set<TEntity>().AsNoTracking().ProjectTo<TDto>(_mapperConfig);
            }
        }
    }
}