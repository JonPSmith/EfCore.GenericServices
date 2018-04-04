// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;

namespace GenericServices.Setup.Internal
{
    internal class CreateConfigGenerator
    {
        public dynamic Accessor { get; }

        public CreateConfigGenerator(Type dtoType, DecodedEntityClass entityInfo, object configInfo)
        {
            var myGeneric = typeof(ConfigGenerator<,>);
            var copierType = myGeneric.MakeGenericType(dtoType, entityInfo.EntityType);
            Accessor = Activator.CreateInstance(copierType, new object[]{ configInfo});
        }

        public class ConfigGenerator<TDto, TEntity>
            where TDto : class
            where TEntity : class
        {
            private readonly PerDtoConfig<TDto, TEntity> _config;

            public ConfigGenerator(PerDtoConfig<TDto, TEntity> config)
            {
                _config = config;
            }

            public void AddReadMappingToProfile(Profile readProfile)
            {
                if (_config?.AlterReadMapping == null)
                    readProfile.CreateMap<TEntity, TDto>();
                else
                {
                    _config.AlterReadMapping(readProfile.CreateMap<TEntity, TDto>());
                }
            }

            public void AddSaveMappingToProfile(Profile writeProfile)
            {
                if (_config?.AlterSaveMapping == null)
                    writeProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                else
                {
                    _config.AlterSaveMapping(writeProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter());
                }
            }
        }
    }
}