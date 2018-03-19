using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup
{
    public interface IGenericServicesSetup
    {
        IServiceCollection Services { get; }

        WrappedAutoMapperConfig AutoMapperConfig { get; }
    }
}