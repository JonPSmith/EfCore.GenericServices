// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.PublicButHidden;
using GenericServices.Startup.Internal;

namespace Tests.Helpers
{
    public static class AutoMapperHelpers
    {

        public static MapperConfiguration CreateSaveConfig<TDto, TEntity>()
        {

            var saveProfile = new MappingProfile(true);
            saveProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            var saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            });
            return saveConfig;
        }

        public static MapperConfiguration CreateReadConfig<TEntity, TDto>(Action<IMappingExpression<TEntity, TDto>> alterMapping)
        {
            var readProfile = new MappingProfile(false);
            alterMapping(readProfile.CreateMap<TEntity, TDto>());
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            });
            return readConfig;
        }

        public static WrappedAutoMapperConfig CreateWrapperMapper<TDto, TEntity>()
        {
            var readProfile = new MappingProfile(false);
            readProfile.CreateMap<TEntity, TDto>();
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            });

            var saveProfile = new MappingProfile(true);
            saveProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            var saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            });
            return new WrappedAutoMapperConfig(readConfig, saveConfig);
        }
    }
}