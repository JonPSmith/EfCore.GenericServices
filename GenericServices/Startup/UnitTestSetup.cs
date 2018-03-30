// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
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
        /// <summary>
        /// This is designed to set up the system for using one DTO in a unit test of a service
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="context"></param>
        /// <param name="publicConfig">WARNING: you MUST use a consistant GenericServicesConfig across ALL your tests.
        /// This is because the configuration is used in cached values. If you want to test a different configuration then
        /// make the test ONLY runnable by hand.</param>
        /// <returns></returns>
        public static IWrappedAutoMapperConfig SetupSingleDtoAndEntities<TDto>(this DbContext context,
            IGenericServicesConfig publicConfig = null)
        {
            var status = new StatusGenericHandler();
            publicConfig = publicConfig ?? new GenericServicesConfig();
            context.RegisterEntityClasses();
            var typesInAssembly = typeof(TDto).Assembly.GetTypes();
            var dtoRegister = new RegisterOneDtoType(typeof(TDto), typesInAssembly, publicConfig);
            status.CombineStatuses(dtoRegister);
            if (!status.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {status.Errors.Count} errors. Errors are:\n" 
                                                    + status.GetAllErrors());

            var readProfile = new MappingProfile(false);
            dtoRegister.ConfigGenerator.Accessor.AddReadMappingToProfile(readProfile);
            var saveProfile = new MappingProfile(true);
            //Only add a mapping if AutoMapper can be used to update/create the entity
            if (dtoRegister.EntityInfo.EntityStyle != EntityStyles.DDDStyled && 
                dtoRegister.EntityInfo.EntityStyle != EntityStyles.ReadOnly)
            {
                dtoRegister.ConfigGenerator.Accessor.AddSaveMappingToProfile(saveProfile);
            }

            return SetupAllDtosAndMappings.CreateWrappedAutoMapperConfig(readProfile, saveProfile);
        }
    }
}