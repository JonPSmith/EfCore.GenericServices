// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;

namespace GenericServices.Internal.MappingCode
{
    internal class CreateMapGenerator
    {
        public dynamic Accessor { get; }

        private IGenericServicesConfig _publicConfig;

        public CreateMapGenerator(Type dtoType, DecodedEntityClass entityInfo, IGenericServicesConfig publicConfig, object configInfo)
        {
            _publicConfig = publicConfig;
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

            public void BuildReadMapping(Profile readProfile)
            {
                if (_config?.AlterReadMapping == null)
                    readProfile.CreateMap<TEntity, TDto>();
                else
                {
                    _config.AlterReadMapping(readProfile.CreateMap<TEntity, TDto>());
                }
            }

            public void BuildSaveMapping(Profile writeProfile)
            {
                if (_config?.AlterSaveMapping == null)
                    writeProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                else
                {
                    _config.AlterSaveMapping(writeProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter());
                }
            }

            public PerDtoConfig GetRestOfPerDtoConfig()
            {
                return _config;
            }
        }
    }
}