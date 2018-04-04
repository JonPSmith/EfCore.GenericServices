// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.Setup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Setup
{
    /// <summary>
    /// This contains an extension method if you want to use GenericServices in a unit test.
    /// It sets up a single DTO with all the entity classes
    /// NOTE because all the setups are cached other tests may have registered other DTOs
    /// </summary>
    public static class UnitTestSetup
    {
        /// <summary>
        /// This is designed to set up the system for using one DTO and the entity classes in a unit test of a service
        /// </summary>
        /// <typeparam name="TDto">This should be the type of a class that has the <see cref="ILinkToEntity{TEntity}"/> applied to it</typeparam>
        /// <param name="context">This is the DbContext conatining the entity clas your TDto refers to</param>
        /// <param name="publicConfig">Optional: you can provide a publicConfig. 
        /// NOTE: All use of this method must use the same config file, because it is read at startup and then cached.</param>
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
            var saveProfile = new MappingProfile(true);
            SetupDtosAndMappings.SetupMappingForDto(dtoRegister, readProfile, saveProfile);
            return SetupDtosAndMappings.CreateWrappedAutoMapperConfig(readProfile, saveProfile);
        }
    }
}