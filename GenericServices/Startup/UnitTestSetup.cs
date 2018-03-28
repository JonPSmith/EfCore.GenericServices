// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration;
using GenericServices.Startup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GenericServices.Configuration.Internal;

namespace GenericServices.Startup
{
    public static class UnitTestSetup
    {
        public static WrappedAutoMapperConfig SetupSingleDtoAndEntities<TDto>(this DbContext context,
            IGenericServicesConfig publicConfig = null)
        {
            var status = new StatusGenericHandler();
            publicConfig = publicConfig ?? new GenericServicesConfig();
            context.RegisterEntityClasses();
            var dtoRegister = new RegisterOneDtoType(typeof(TDto), new ExpandedGlobalConfig( publicConfig, context));
            status.CombineStatuses(dtoRegister);
            if (!status.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {status.Errors.Count}. Errors are:\n" 
                                                    + string.Join("\n", status.Errors.Select(x => x.ToString())));

            var readProfile = new MappingProfile(false);
            dtoRegister.MapGenerator.Accessor.BuildReadMapping(readProfile);
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            });
            var saveProfile = new MappingProfile(true);
            dtoRegister.MapGenerator.Accessor.BuildSaveMapping(saveProfile);
            var saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            });

            return new WrappedAutoMapperConfig(readConfig, saveConfig);
        }
    }
}