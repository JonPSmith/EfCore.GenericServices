// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.Setup.Internal;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace GenericServices.Setup
{
    /// <summary>
    /// This contains an extension method if you want to use GenericServices in a unit test or non-DI situation
    /// It sets up a single DTO with all the entity classes
    /// NOTE because all the setups are cached other tests may have registered other DTOs
    /// </summary>
    public static class UnitTestSetup
    {
        /// <summary>
        /// This is designed to set up the system for using direct access entity classes in a unit test or a service
        /// </summary>
        /// <param name="context">This is the DbContext conatining the entity clas your TDto refers to</param>
        /// <param name="publicConfig">Optional: you can provide a publicConfig. 
        /// NOTE: All use of this method must use the same config file, because it is read at startup and then cached.</param>
        public static SpecificUseData SetupEntitiesDirect(this DbContext context,
            IGenericServicesConfig publicConfig = null)
        {
            context.RegisterEntityClasses();
            var utData = new SpecificUseData(publicConfig);
            return utData;
        }

        /// <summary>
        /// This is designed to set up the system for using one DTO and the entity classes in a unit test or a service
        /// </summary>
        /// <typeparam name="TDto">This should be the type of a class that has the <see cref="ILinkToEntity{TEntity}"/> applied to it</typeparam>
        /// <param name="context">This is the DbContext containing the entity class your TDto refers to</param>
        /// <param name="publicConfig">Optional: you can provide a publicConfig. 
        /// NOTE: All use of this method must use the same config file, because it is read at startup and then cached.</param>
        public static SpecificUseData SetupSingleDtoAndEntities<TDto>(this DbContext context,
            IGenericServicesConfig publicConfig = null)
        {
            context.RegisterEntityClasses();
            var utData = new SpecificUseData(publicConfig);
            utData.AddSingleDto<TDto>();
            return utData;
        }

        /// <summary>
        /// This is designed to add one DTO to an existing SpecificUseData
        /// </summary>
        /// <typeparam name="TDto">This should be the type of a class that has the <see cref="ILinkToEntity{TEntity}"/> applied to it</typeparam>
        /// <param name="utData"></param>
        /// <returns></returns>
        public static SpecificUseData AddSingleDto<TDto>(this SpecificUseData utData)
        {
            var status = new StatusGenericHandler();
            var typesInAssembly = typeof(TDto).Assembly.GetTypes();
            var dtoRegister = new RegisterOneDtoType(typeof(TDto), typesInAssembly, utData.PublicConfig);
            status.CombineStatuses(dtoRegister);
            if (!status.IsValid)
                throw new InvalidOperationException($"SETUP FAILED with {status.Errors.Count} errors. Errors are:\n"
                                                    + status.GetAllErrors());

            SetupDtosAndMappings.SetupMappingForDto(dtoRegister, utData.ReadProfile, utData.SaveProfile);
            return utData;
        }
    }
}