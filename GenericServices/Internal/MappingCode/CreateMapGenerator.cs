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

        private DecodedDto _dtoInfo;
        private IGenericServiceConfig _configuration;

        public CreateMapGenerator(DecodedDto dtoInfo, DecodedEntityClass entityInfo, IGenericServiceConfig configuration, object configInfo)
        {
            _dtoInfo = dtoInfo;
            _configuration = configuration;
            var myGeneric = typeof(MapGenerator<,>);
            var copierType = myGeneric.MakeGenericType(dtoInfo.DtoType, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(copierType, new object[]{ configInfo});
        }

        public class MapGenerator<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly DtoConfigInfo<TDto, TEntity> _configInfo;

            public MapGenerator(DtoConfigInfo<TDto, TEntity> configInfo)
            {
                _configInfo = configInfo;
            }

            public void BuildReadMapping(IMapperConfigurationExpression cfg)
            {
                if (_configInfo?.AlterReadMapping == null)
                    cfg.CreateMap<TEntity, TDto>();
                else
                {
                    _configInfo.AlterReadMapping(cfg.CreateMap<TEntity, TDto>());
                }
            }

            public void BuildSaveMapping(IMapperConfigurationExpression cfg)
            {
                if (_configInfo?.AlterSaveMapping == null)
                    cfg.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                else
                {
                    _configInfo.AlterReadMapping(cfg.CreateMap<TEntity, TDto>().IgnoreAllPropertiesWithAnInaccessibleSetter());
                }
            }
        }
    }
}