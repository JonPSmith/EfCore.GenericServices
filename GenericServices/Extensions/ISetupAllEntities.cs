using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Extensions
{
    public interface ISetupAllEntities
    {
        IServiceCollection Services { get; }
    }
}