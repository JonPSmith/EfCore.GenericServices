// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using AutoMapper;
using GenericServices.Internal.Decoders;
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
            var copierType = myGeneric.MakeGenericType(tDto, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(copierType, new object[] { context, wrapperMapperConfigs});
        }

        public class GenericMapper<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DbContext _context;
            private readonly IWrappedAutoMapperConfig _wrapperMapperConfigs;

            public GenericMapper(DbContext context, IWrappedAutoMapperConfig wrapperMapperConfigs)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _wrapperMapperConfigs = wrapperMapperConfigs ?? throw new ArgumentNullException(nameof(wrapperMapperConfigs));
            }
                
            public void MapDtoToEntity(TDto dto, object entity)
            {
                _wrapperMapperConfigs.MapperSaveConfig.CreateMapper().Map(dto, entity);
            }

            public TEntity ReturnExistingEntity(object[] keys)
            {
                return _context.Set<TEntity>().Find(keys);
            }
        }
    }
}