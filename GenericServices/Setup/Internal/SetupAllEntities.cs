// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Setup.Internal
{
    internal class SetupAllEntities : IGenericServicesSetupPart1
    {
        public SetupAllEntities(IServiceCollection services, IGenericServicesConfig publicConfig, Type[] contextTypes)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            PublicConfig = publicConfig ?? new GenericServicesConfig();
            if (contextTypes == null || contextTypes.Length <= 0)
                throw new ArgumentException(nameof(contextTypes));

            using var serviceProvider = services.BuildServiceProvider();
            var serviceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                foreach (var contextType in contextTypes)
                {
                    using (var context = serviceScope.ServiceProvider.GetService(contextType) as DbContext)
                    {
                        if (context == null)
                            throw new InvalidOperationException($"You provided the a DbContext called {contextType.Name}, but it doesn't seem to be registered, or is a DbContext. Have you forgotten to register it?");
                        context.RegisterEntityClasses();
                    }
                }
            }
        }

        public IGenericServicesConfig PublicConfig { get; }
        public IServiceCollection Services { get; }
    }
}
