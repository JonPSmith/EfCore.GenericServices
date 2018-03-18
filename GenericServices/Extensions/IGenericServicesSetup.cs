using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Extensions
{
    public interface IGenericServicesSetup
    {
        IServiceCollection Services { get; }

        WrappedAutoMapperConfig AutoMapperConfig { get; }
    }
}