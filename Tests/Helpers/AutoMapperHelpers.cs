// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.PublicButHidden;

namespace Tests.Helpers
{
    public static class AutoMapperHelpers
    {

        public static MapperConfiguration CreateSaveConfig<TDto, TEntity>()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                });
            return config;
        }

        public static MapperConfiguration CreateReadConfig<TEntity, TDto>(Action<IMappingExpression<TEntity, TDto>> alterMapping)
        {
            var config = new MapperConfiguration(cfg =>
            {
                alterMapping(cfg.CreateMap<TEntity, TDto>());
            });
            return config;
        }

        public static WrappedAutoMapperConfig CreateWrapperMapper<TDto, TEntity>()
        {
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TEntity, TDto>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            });
            var saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            });
            return new WrappedAutoMapperConfig(readConfig, saveConfig);
        }
    }
}