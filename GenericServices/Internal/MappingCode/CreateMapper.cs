// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper.QueryableExtensions;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.LinqBuilders;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class CreateMapper
    {
        public dynamic Accessor { get;  }

        public CreateMapper(DbContext context, IWrappedAutoMapperConfig wrapperMapperConfigs, Type tDto, DecodedEntityClass entityInfo)
        {
            var myGeneric = typeof(GenericMapper<,>);
            var genericType = myGeneric.MakeGenericType(tDto, entityInfo.EntityType);
            var constructor = genericType.GetConstructors().Single();
            Accessor = GetNewGenericMapper(genericType, constructor).Invoke(context, wrapperMapperConfigs, entityInfo);
            //Using Activator.CreateInstance with dynamic takes twice as long as LINQ new - see TestNewCreateMapper
            //Accessor = Activator.CreateInstance(genericType, context, wrapperMapperConfigs, entityInfo);
        }

        private static readonly ConcurrentDictionary<Type, dynamic> NewGenericMapperCache = new ConcurrentDictionary<Type, dynamic>();

        //This is only public for performance tests
        public static dynamic GetNewGenericMapper(Type genericType, ConstructorInfo ctor)
        {
            return NewGenericMapperCache.GetOrAdd(genericType, value => NewGenericMapper(ctor));
        }

        //This is only public for performance tests
        public static dynamic NewGenericMapper(ConstructorInfo ctor)
        {
            var arg1 = Expression.Parameter(typeof(DbContext), "context");
            var arg2 = Expression.Parameter(typeof(IWrappedAutoMapperConfig), "wrapperMapper");
            var arg3 = Expression.Parameter(typeof(DecodedEntityClass), "entityInfo");
            var newExp = Expression.New(ctor, arg1, arg2, arg3);
            var built = Expression.Lambda(newExp, false, arg1, arg2, arg3);
            return built.Compile();
        }

        public class GenericMapper<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DbContext _context;
            private readonly IWrappedAutoMapperConfig _wrappedMapper;
            private readonly DecodedEntityClass _entityInfo;

            public string EntityName => _entityInfo.EntityType.Name;

            public GenericMapper(DbContext context, IWrappedAutoMapperConfig wrappedMapper, DecodedEntityClass entityInfo)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _wrappedMapper = wrappedMapper ?? throw new ArgumentNullException(nameof(wrappedMapper));
                _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            }

            public void MapDtoToEntity(TDto dto, object entity)
            {
                _wrappedMapper.MapperSaveConfig.CreateMapper().Map(dto, entity);
            }

            public TEntity ReturnExistingEntity(object[] keys)
            {
                return _context.Set<TEntity>().Find(keys);
            }

            public IQueryable<TDto> GetViaKeysWithProject(params object[] keys)
            {
                var predicate = _entityInfo.PrimaryKeyProperties.CreateFilter<TEntity>(keys);
                return _context.Set<TEntity>().Where(predicate).ProjectTo<TDto>(_wrappedMapper.MapperReadConfig);
            }

            public IQueryable<TDto> ProjectAndThenApplyWhereExpression(Expression<Func<TDto, bool>> whereExpression)
            {
                return _context.Set<TEntity>().ProjectTo<TDto>(_wrappedMapper.MapperReadConfig).Where(whereExpression);
            }

            public IQueryable<TDto> GetManyProjectedNoTracking()
            {
                return _context.Set<TEntity>().AsNoTracking().ProjectTo<TDto>(_wrappedMapper.MapperReadConfig);
            }
        }
    }
}