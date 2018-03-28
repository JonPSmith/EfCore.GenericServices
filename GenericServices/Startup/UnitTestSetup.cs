// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using GenericServices.Configuration;
using GenericServices.Startup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;

namespace GenericServices.Startup
{
    public static class UnitTestSetup
    {
        public static WrappedAutoMapperConfig SetupSingleDtoAndEntities<TDto>(this DbContext context,
            IGenericServicesConfig publicConfig = null)
        {
            var status = new StatusGenericHandler();
            publicConfig = publicConfig ?? new GenericServicesConfig();
            var dtoRegister = new RegisterOneDtoType(typeof(TDto), new ExpandedGlobalConfig( publicConfig, context));
            status.CombineStatuses(dtoRegister);
            if (!status.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {status.Errors.Count} errors. Errors are:\n" 
                                                    + status.GetAllErrors());

            var readProfile = new MappingProfile(false);
            dtoRegister.MapGenerator.Accessor.BuildReadMapping(readProfile);
            var readConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            });
            var saveProfile = new MappingProfile(true);
            //Only add a mapping if AutoMapper can be used to update/create the entity
            if (dtoRegister.EntityInfo.EntityStyle != EntityStyles.DDDStyled && 
                dtoRegister.EntityInfo.EntityStyle != EntityStyles.ReadOnly)
            {
                dtoRegister.MapGenerator.Accessor.BuildSaveMapping(saveProfile);
            }
            var saveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            });

            return new WrappedAutoMapperConfig(readConfig, saveConfig);
        }
    }
}