// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration;

namespace GenericServices.PublicButHidden
{
    /// <summary>
    /// This is the interface used for dependency injection of the <see cref="WrappedAndMapper"/>
    /// </summary>
    public interface IWrappedConfigAndMapper
    {
        /// <summary>
        /// This is the global configuration information
        /// </summary>
        IGenericServicesConfig Config { get; }

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
    public class WrappedAndMapper : IWrappedConfigAndMapper
    {
        internal WrappedAndMapper(IGenericServicesConfig config, MapperConfiguration mapperReadConfig, MapperConfiguration mapperSaveConfig)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
            MapperReadConfig = mapperReadConfig ?? throw new ArgumentNullException(nameof(mapperReadConfig));
            MapperSaveConfig = mapperSaveConfig ?? throw new ArgumentNullException(nameof(mapperSaveConfig));
        }

        /// <inheritdoc />
        public IGenericServicesConfig Config { get; }

        /// <inheritdoc />
        public MapperConfiguration MapperReadConfig { get; }

        /// <inheritdoc />
        public MapperConfiguration MapperSaveConfig { get; }
    }
}