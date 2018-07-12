using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Setup
{
    /// <summary>
    /// Used to chain ConfigureGenericServicesEntities to RegisterGenericServices
    /// </summary>
    public interface IGenericServicesSetupPart2 
    {
        /// <summary>
        /// The DI ServiceCollection to use for registering
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Global GenericServices configuration
        /// </summary>
        IGenericServicesConfig PublicConfig { get; }

        /// <summary>
        /// The AutoMapper setting needed by GenericServices
        /// </summary>
        IWrappedConfigAndMapper ConfigAndMapper { get; }
    }
}