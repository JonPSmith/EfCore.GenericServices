// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.Configuration;
using GenericServices.Extensions.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Extensions
{
    public static class ConfigureGenericServices
    {
        /// <summary>
        /// This will configure GenericServices if you are using one DbContext and you are happy to use the default GenericServices configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection GenericServicesSimpleSetup(this IServiceCollection services,
            params Assembly[] assemblies)
        {
            return services.ConfigureGenericServicesEntities()
                .ScanAssemblesForDtos(assemblies)
                .RegisterGenericServices();
        }

        public static IGenericServicesSetup ConfigureGenericServicesEntities(this IServiceCollection serviceCollection,
            params Type[] contextTypes)
        {
            return serviceCollection.ConfigureGenericServicesEntities(null, contextTypes);
        }

        public static IGenericServicesSetup ConfigureGenericServicesEntities(this IServiceCollection services,
            IGenericServiceConfig configuration, params Type[] contextTypes)
        {
            if (contextTypes.Length == 0)
            {
                contextTypes = new[] { typeof(DbContext) };
            }

            var setupEntities = new SetupAllEntities(services, configuration, contextTypes);

            return setupEntities;
        }

        public static IGenericServicesSetup ScanAssemblesForDtos(this IGenericServicesSetup genericServicesSetup,
            params Assembly[] assemblies)
        {
            assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();

            return genericServicesSetup;
        }

        public static IServiceCollection RegisterGenericServices(this IGenericServicesSetup genericServicesSetup)
        {
            //services.AddScoped<IGenericService, GenericService>();
            genericServicesSetup.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            //Async to go here

            //Register AutoMapper configuration goes here


            return genericServicesSetup.Services;
        }
    }
}