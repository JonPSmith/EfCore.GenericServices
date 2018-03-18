// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Extensions
{
    public static class DependencyInjection
    {

        public static IServiceCollection RegisterGenericServices(this IServiceCollection services)
        {
            //services.AddScoped<IGenericService, GenericService>();
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            //Async to go here

            //Register AutoMapper configuration
            return services;
        }
    }
}