// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.Configuration;
using GenericServices.Setup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Setup
{
    /// <summary>
    /// This contains extension methods for setting up GenericServices at startup.
    /// It assumes the use of dependency injection (DI) and <see cref="IServiceCollection"/> for DI registering
    /// </summary>
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


        /// <summary>
        /// This will configure GenericServices if you are using one DbContext and you are happy to use the default GenericServices configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="assembliesToScan">You can define the assemblies to scan for DTOs/ViewModels. Otherwise it will scan all assemblies (slower, but simple)</param>
        /// <returns></returns>
        public static IServiceCollection GenericServicesSimpleSetup<TContext>(this IServiceCollection services,
                    IGenericServicesConfig configuration, params Assembly[] assembliesToScan) where TContext : DbContext
        {
            return services.ConfigureGenericServicesEntities(configuration, typeof(TContext))
                .ScanAssemblesForDtos(assembliesToScan)
                .RegisterGenericServices(typeof(TContext));
        }

        /// <summary>
        /// If you want to use multiple DbContexts then you should use this, plus <see cref="ScanAssemblesForDtos"/> and <see cref="RegisterGenericServices"/>
        /// to set up GenericServices
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="contextTypes">You should provide an array of your DbContext types that you want to register</param>
        /// <returns></returns>
        public static IGenericServicesSetupPart1 ConfigureGenericServicesEntities(this IServiceCollection serviceCollection,
            params Type[] contextTypes)
        {
            return serviceCollection.ConfigureGenericServicesEntities(null, contextTypes);
        }

        /// <summary>
        /// If you want to use multiple DbContexts then you should use this, plus <see cref="ScanAssemblesForDtos"/> and <see cref="RegisterGenericServices"/>
        /// to set up GenericServices. This version allow you to provide a <see cref="GenericServicesConfig"/> with your settings
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration">You provide a <see cref="GenericServicesConfig"/> with your settings</param>
        /// <param name="contextTypes">You should provide an array of your DbContext types that you want to register</param>
        /// <returns></returns>
        public static IGenericServicesSetupPart1 ConfigureGenericServicesEntities(this IServiceCollection services,
            IGenericServicesConfig configuration, params Type[] contextTypes)
        {
            var setupEntities = new SetupAllEntities(services, configuration, contextTypes);

            return setupEntities;
        }

        /// <summary>
        /// If you use ConfigureGenericServicesEntities, then you should follow it with this method to find/set up the DTOs
        /// </summary>
        /// <param name="setupPart1"></param>
        /// <param name="assembliesToScan"></param>
        /// <returns></returns>
        public static IGenericServicesSetupPart2 ScanAssemblesForDtos(this IGenericServicesSetupPart1 setupPart1,
            params Assembly[] assembliesToScan)
        {
            var dtosRegister = new SetupDtosAndMappings(setupPart1.PublicConfig);
            var wrappedMapping = dtosRegister.ScanAllAssemblies(assembliesToScan, setupPart1.PublicConfig);
            if (!dtosRegister.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {dtosRegister.Errors.Count} errors. Errors are:\n"
                                                    + dtosRegister.GetAllErrors());

            return new GenericServicesSetupPart2(setupPart1.Services, setupPart1.PublicConfig, wrappedMapping);
        }

        /// <summary>
        /// If you used ScanAssemblesForDtos you should add this method on the end
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
            setupPart2.Services.AddTransient(typeof(ICrudServices<>), typeof(CrudServices<>));
            setupPart2.Services.AddTransient(typeof(ICrudServicesAsync<>), typeof(CrudServicesAsync<>));

            //If there is only one DbContext then the developer can use the non-generic CrudServices
            if (singleContextToRegister != null)
            {
                setupPart2.Services.AddTransient<ICrudServices, CrudServices>();
                setupPart2.Services.AddTransient<ICrudServicesAsync, CrudServicesAsync>();
                setupPart2.Services.AddTransient(s => (DbContext)s.GetRequiredService(singleContextToRegister));
            }

            //Register AutoMapper configuration goes here
            setupPart2.Services.AddSingleton(setupPart2.ConfigAndMapper);

            return setupPart2.Services;
        }
    }
}