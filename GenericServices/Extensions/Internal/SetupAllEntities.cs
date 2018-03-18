// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using GenericLibsBase;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Extensions.Internal
{
    internal class SetupAllEntities : StatusGenericHandler, IGenericServicesSetup
    {
        private IGenericServiceConfig _configuration;

        public IServiceCollection Services { get; }
        public WrappedAutoMapperConfig AutoMapperConfig { get;}

        public SetupAllEntities(IServiceCollection services, IGenericServiceConfig configuration, Type[] contextTypes)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _configuration = configuration ?? new GenericServicesConfig();
            if (contextTypes == null || contextTypes.Length < 0)
                throw new ArgumentException(nameof(contextTypes));

            var serviceProvider = services.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                foreach (var contextType in contextTypes)
                {
                    using (var context = serviceScope.ServiceProvider.GetService(contextType) as DbContext)
                    {
                        if (context == null)
                            throw new InvalidOperationException($"You provided the a DbContext called {contextType.Name}, but it doesn't seem to be registered. Have you forgotten to register it?");
                        CombineErrors(RegisterEntityClasses(context));
                    }
                }
            }
        }

        public static IStatusGeneric RegisterEntityClasses(DbContext context)
        {
            var status = new StatusGenericHandler();
            var entityNameList = new List<string>();
            foreach (var entityType in context.Model.GetEntityTypes())
            {
                var entityInfo = context.RegisterDecodedEntityClass(entityType.ClrType);
                entityNameList.Add(entityInfo.EntityType.Name);
            }

            status.Message = "Entity types: " + string.Join(", ", entityNameList);
            return status;
        }
    }
}