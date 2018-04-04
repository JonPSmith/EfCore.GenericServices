// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.Configuration;
using GenericServices.Startup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup
{
    public static class ConfigureGenericServices
    {
        /// <summary>
        /// This will configure GenericServices if you are using one DbContext and you are happy to use the default GenericServices configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembliesToScan">You can define the assemblies to scan for DTOs/ViewModels. Otherwise it will scan all assemblies (slower, but simple)</param>
        /// <returns></returns>
        public static IServiceCollection GenericServicesSimpleSetup<TContext>(this IServiceCollection services,
            params Assembly[] assembliesToScan) where TContext : DbContext
        {
            return services.ConfigureGenericServicesEntities(typeof(TContext))
                .ScanAssemblesForDtos(assembliesToScan)
                .RegisterGenericServices(typeof(TContext));
        }

        public static IGenericServicesSetupPart1 ConfigureGenericServicesEntities(this IServiceCollection serviceCollection,
            params Type[] contextTypes)
        {
            return serviceCollection.ConfigureGenericServicesEntities(null, contextTypes);
        }

        public static IGenericServicesSetupPart1 ConfigureGenericServicesEntities(this IServiceCollection services,
            IGenericServicesConfig configuration, params Type[] contextTypes)
        {
            var setupEntities = new SetupAllEntities(services, configuration, contextTypes);

            return setupEntities;
        }

        public static IGenericServicesSetupPart2 ScanAssemblesForDtos(this IGenericServicesSetupPart1 setupPart1,
            params Assembly[] assembliesToScan)
        {
            var dtosRegister = new SetupDtosAndMappings(setupPart1.PublicConfig);
            var wrappedMapping = dtosRegister.ScanAllAssemblies(assembliesToScan, true);
            if (!dtosRegister.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {dtosRegister.Errors.Count} errors. Errors are:\n"
                                                    + dtosRegister.GetAllErrors());

            return new GenericServicesSetupPart2(setupPart1.Services, setupPart1.PublicConfig, wrappedMapping);
        }

        /// <summary>
        /// This registers all the services needed to run GenericServices. You will be able to access GenericServices
        /// via its interfaces: ICrudServices, <see cref="ICrudServices{TContext}" /> and async versions
        /// </summary>
        /// <param name="setupPart2"></param>
        /// <param name="singleContextToRegister">If you have one DbContext and you want to use the non-generic ICrudServices
        /// then GenericServices has to register your DbContext against your application's DbContext</param>
        /// <returns></returns>
        public static IServiceCollection RegisterGenericServices(this IGenericServicesSetupPart2 setupPart2, 
            Type singleContextToRegister = null)
        {
            setupPart2.Services.AddScoped(typeof(ICrudServices<>), typeof(CrudServices<>));
            setupPart2.Services.AddScoped(typeof(ICrudServicesAsync<>), typeof(CrudServicesAsync<>));

            //If there is only one DbContext then the developer can use the non-generic CrudServices
            if (singleContextToRegister != null)
            {
                setupPart2.Services.AddScoped<ICrudServices, CrudServices>();
                setupPart2.Services.AddScoped<ICrudServicesAsync, CrudServicesAsync>();
                setupPart2.Services.AddScoped(s => (DbContext)s.GetRequiredService(singleContextToRegister));
            }

            //Register AutoMapper configuration goes here
            setupPart2.Services.AddSingleton(setupPart2.AutoMapperConfig);

            return setupPart2.Services;
        }
    }
}