// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup.Internal
{
    public class GenericServicesSetupPart2 : IGenericServicesSetupPart2
    {
        public GenericServicesSetupPart2(IServiceCollection services, IGenericServicesConfig publicConfig, IWrappedAutoMapperConfig autoMapperConfig)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            PublicConfig = publicConfig ?? throw new ArgumentNullException(nameof(publicConfig));
            AutoMapperConfig = autoMapperConfig ?? throw new ArgumentNullException(nameof(autoMapperConfig));
        }

        public IServiceCollection Services { get; }
        public IGenericServicesConfig PublicConfig { get; }
        public IWrappedAutoMapperConfig AutoMapperConfig { get; }
    }
}