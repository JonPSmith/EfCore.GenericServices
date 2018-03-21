// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.PublicButHidden
{
    public interface IWrappedAutoMapperConfig
    {
        MapperConfiguration MapperReadConfig { get; }
        MapperConfiguration MapperSaveConfig { get; }
    }

    public class WrappedAutoMapperConfig : IWrappedAutoMapperConfig
    {
        public WrappedAutoMapperConfig(MapperConfiguration mapperReadConfig, MapperConfiguration mapperSaveConfig)
        {
            MapperReadConfig = mapperReadConfig ?? throw new ArgumentNullException(nameof(mapperReadConfig));
            MapperSaveConfig = mapperSaveConfig ?? throw new ArgumentNullException(nameof(mapperSaveConfig));
        }

        public MapperConfiguration MapperReadConfig { get; }

        public MapperConfiguration MapperSaveConfig { get; }
    }
}