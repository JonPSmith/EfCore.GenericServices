// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;

namespace GenericServices.Internal.MappingCode
{
    internal class CreateMapGenerator
    {
        public dynamic Accessor { get; }

        private IGenericServiceConfig _configuration;

        public CreateMapGenerator(Type dtoType, DecodedEntityClass entityInfo, IGenericServiceConfig configuration, object configInfo)
        {
            _configuration = configuration;
            var myGeneric = typeof(MapGenerator<,>);
            var copierType = myGeneric.MakeGenericType(dtoType, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(copierType, new object[]{ configInfo});
        }

        public class MapGenerator<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly PerDtoConfig<TDto, TEntity> _config;

            public MapGenerator(PerDtoConfig<TDto, TEntity> config)
            {
                _config = config;
            }

            public void BuildReadMapping(IMapperConfigurationExpression cfg)
            {
                if (_config?.AlterReadMapping == null)
                    cfg.CreateMap<TEntity, TDto>();
                else
                {
                    _config.AlterReadMapping(cfg.CreateMap<TEntity, TDto>());
                }
            }

            public void BuildSaveMapping(IMapperConfigurationExpression cfg)
            {
                if (_config?.AlterSaveMapping == null)
                    cfg.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                else
                {
                    _config.AlterSaveMapping(cfg.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter());
                }
            }

            public PerDtoConfig GetRestOfPerDtoConfig()
            {
                return _config;
            }
        }
    }
}