// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.Extensions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Extensions
{
    public static class ConfigureGenericServices
    {
        public static ISetupAllEntities ConfigureGenericServicesEntities(this IServiceCollection serviceCollection,
            params Type[] contextTypes)
        {
            return serviceCollection.ConfigureGenericServicesEntities(null, contextTypes);
        }

        public static ISetupAllEntities ConfigureGenericServicesEntities(this IServiceCollection services,
            IGenericServiceConfig configuration, params Type[] contextTypes)
        {
            if (contextTypes.Length == 0)
            {
                contextTypes = new[] { typeof(DbContext) };
            }

            var setupEntities = new SetupAllEntities(services, configuration, contextTypes);

            return setupEntities;
        }

        public static IServiceCollection ConfigureServicesDtos(this ISetupAllEntities setupEntities,
            params Assembly[] assemblies)
        {

            return setupEntities.Services;
        }
    }
}