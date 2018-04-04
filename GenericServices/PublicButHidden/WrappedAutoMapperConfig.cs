// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.PublicButHidden
{
    /// <summary>
    /// This is the interface used for dependency injection of the <see cref="WrappedAutoMapperConfig"/>
    /// </summary>
    public interface IWrappedAutoMapperConfig
    {
        /// <summary>
        /// This is the AutoMapper configuration used for reading/projection from entity class to DTO
        /// </summary>
        MapperConfiguration MapperReadConfig { get; }

        /// <summary>
        /// This is the AutoMapper configuration used for copying from a DTO to the entity class
        /// </summary>
        MapperConfiguration MapperSaveConfig { get; }
    }

    /// <summary>
    /// This contains the AutoMapper setting needed by GenericServices
    /// </summary>
    public class WrappedAutoMapperConfig : IWrappedAutoMapperConfig
    {
        internal WrappedAutoMapperConfig(MapperConfiguration mapperReadConfig, MapperConfiguration mapperSaveConfig)
        {
            MapperReadConfig = mapperReadConfig ?? throw new ArgumentNullException(nameof(mapperReadConfig));
            MapperSaveConfig = mapperSaveConfig ?? throw new ArgumentNullException(nameof(mapperSaveConfig));
        }

        /// <inheritdoc />
        public MapperConfiguration MapperReadConfig { get; }

        /// <inheritdoc />
        public MapperConfiguration MapperSaveConfig { get; }
    }
}