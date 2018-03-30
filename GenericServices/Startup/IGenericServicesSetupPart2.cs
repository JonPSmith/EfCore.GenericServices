using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup
{
    public interface IGenericServicesSetupPart2 
    {
        IServiceCollection Services { get; }
        IWrappedAutoMapperConfig AutoMapperConfig { get; }
    }
}