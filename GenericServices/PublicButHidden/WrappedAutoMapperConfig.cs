// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.PublicButHidden
{
    public interface IWrappedAutoMapperConfig
    {
        MapperConfiguration AutoMapperConfig { get; }
    }

    public class WrappedAutoMapperConfig : IWrappedAutoMapperConfig
    {
        public WrappedAutoMapperConfig(MapperConfiguration mapperConfig)
        {
            AutoMapperConfig = mapperConfig ?? throw new ArgumentNullException(nameof(mapperConfig));
        }

        public MapperConfiguration AutoMapperConfig { get; }


    }
}