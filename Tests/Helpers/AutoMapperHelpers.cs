// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.PublicButHidden;

namespace Tests.Helpers
{
    public static class AutoMapperHelpers
    {

        public static MapperConfiguration CreateConfig<TIn, TOut>()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TIn, TOut>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                });
            return config;
        }

        public static MapperConfiguration CreateReadConfigWithConfig<TIn, TOut>(Action<IMappingExpression<TIn, TOut>> alterMapping)
        {
            var config = new MapperConfiguration(cfg =>
            {
                alterMapping(cfg.CreateMap<TIn, TOut>());
            });
            return config;
        }

        public static WrappedAutoMapperConfig CreateWrapperMapper<TIn, TOut>()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TIn, TOut>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            });
            return new WrappedAutoMapperConfig(config);
        }
    }
}