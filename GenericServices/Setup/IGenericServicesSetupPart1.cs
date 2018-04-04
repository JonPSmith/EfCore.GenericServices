using System;
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