using System;
using GenericServices.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup
{
    public interface IGenericServicesSetupPart1
    {
        IGenericServicesConfig PublicConfig { get; }

        IServiceCollection Services { get; }
    }
}