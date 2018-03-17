// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;
using GenericServices.PublicButHidden;

namespace Tests.Helpers
{
    public static class AutoMapperHelpers
    {

        public static IMapper CreateMapper<TIn, TOut>()
        {
            var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TIn, TOut>().IgnoreAllPropertiesWithAnInaccessibleSetter();
                });
            var mapper = config.CreateMapper();
            return mapper;
        }

        public static WrappedIMapper CreateWrapperMapper<TIn, TOut>()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TIn, TOut>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            });
            var mapper = config.CreateMapper();
            return new WrappedIMapper(mapper);
        }
    }
}