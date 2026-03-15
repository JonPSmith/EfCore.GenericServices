// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Tests.Helpers
{
    public static class AutoMapperHelpers
    {
        public static MapperConfiguration CreateReadConfig<TEntity, TDto>(Action<IMappingExpression<TEntity, TDto>> alterMapping, ILoggerFactory loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;

            var readProfile = new MappingProfile(false);
            alterMapping(readProfile.CreateMap<TEntity, TDto>());
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            },
            loggerFactory);
            return readConfig;
        }

        public static WrappedAndMapper CreateWrapperMapper<TDto, TEntity>(ILoggerFactory loggerFactory = null)
        {
            loggerFactory ??= NullLoggerFactory.Instance;

            var readProfile = new MappingProfile(false);
            readProfile.CreateMap<TEntity, TDto>();
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            }, loggerFactory);

            var saveProfile = new MappingProfile(true);
            saveProfile.CreateMap<TDto, TEntity>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            var saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            }, loggerFactory);
            return new WrappedAndMapper(new GenericServicesConfig(), readConfig, saveConfig);
        }
    }
}