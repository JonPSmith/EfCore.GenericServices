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
        /// <param name="assemblies">You can define the assemblies to scan for DTOs/ViewModels. Otherwise it will scan all assemblies (slower, but simple)</param>
        /// <returns></returns>
        public static IServiceCollection GenericServicesSimpleSetup<TContext>(this IServiceCollection services,
            params Assembly[] assemblies) where TContext : DbContext
        {
            return services.ConfigureGenericServicesEntities(typeof(TContext))
                .ScanAssemblesForDtos(assemblies)
                .RegisterGenericServices(true);
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

        public static IGenericServicesSetupPart1 ScanAssemblesForDtos(this IGenericServicesSetupPart1 genericServicesSetupPart1,
            params Assembly[] assemblies)
        {
            assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            return genericServicesSetupPart1;
        }

        /// <summary>
        /// This registers all the services needed to run GenericServices. You will be able to access GenericServices
        /// via its interfaces: IGenericService and cref="IGenericService<TContext>">
        /// </summary>
        /// <param name="genericServicesSetupPart1"></param>
        /// <param name="registerDbContext">If you have one DbContext and you want to use the non-generic IGenericService
        /// then GenericServices has to register DbContext against your application's DbContext</param>
        /// <returns></returns>
        public static IServiceCollection RegisterGenericServices(this IGenericServicesSetupPart1 genericServicesSetupPart1, 
            bool registerDbContext = false)
        {
            //services.AddScoped<IGenericService, GenericService>();
            genericServicesSetupPart1.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            //Async to go here

            //Register AutoMapper configuration goes here


            return genericServicesSetupPart1.Services;
        }




    }
}