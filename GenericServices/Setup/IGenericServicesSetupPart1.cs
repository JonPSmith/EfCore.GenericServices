// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericServices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Setup
{
    /// <summary>
    /// Used to chain ConfigureGenericServicesEntities to ConfigureGenericServicesEntities
    /// </summary>
    public interface IGenericServicesSetupPart1
    {
        /// <summary>
        /// Global GenericServices configuration
        /// </summary>
        IGenericServicesConfig PublicConfig { get; }

        /// <summary>
        /// The DI ServiceCollection to use for registering
        /// </summary>
        IServiceCollection Services { get; }
    }
}