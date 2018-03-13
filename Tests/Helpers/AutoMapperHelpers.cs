// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using AutoMapper;

namespace Tests.Helpers
{
    public static class AutoMapperHelpers
    {
        public static IMapper CreateMapper<T>() where T : Profile, new()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new T());
            });
            var mapper = config.CreateMapper();
            return mapper;
        }

        public static IMapper CreateMapper<T1, T2>() where T1 : Profile, new() where T2 : Profile, new()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new T1());
                cfg.AddProfile(new T2());
            });
            var mapper = config.CreateMapper();
            return mapper;
        }
    }
}