// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Setup.Internal
{
    /// <summary>
    /// Used to chain ConfigureGenericServicesEntities to RegisterGenericServices
    /// </summary>
    public class GenericServicesSetupPart2 : IGenericServicesSetupPart2
    {

        internal GenericServicesSetupPart2(IServiceCollection services, IGenericServicesConfig publicConfig, IWrappedAutoMapperConfig autoMapperConfig)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            PublicConfig = publicConfig ?? throw new ArgumentNullException(nameof(publicConfig));
            AutoMapperConfig = autoMapperConfig ?? throw new ArgumentNullException(nameof(autoMapperConfig));
        }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        /// <inheritdoc />
        public IGenericServicesConfig PublicConfig { get; }

        /// <inheritdoc />
        public IWrappedAutoMapperConfig AutoMapperConfig { get; }
    }
}