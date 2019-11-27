// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace GenericServices.Configuration
{
    /// <summary>
    /// This is the signature for the method you can add which will be called before SaveChanges
    /// </summary>
    /// <param name="context">Access to DbContext</param>
    /// <returns></returns>
    public delegate IStatusGeneric BeforeSaveChanges(DbContext context);

    /// <summary>
    /// This is the interface for global configuration for GenericServices. 
    /// </summary>
    public interface IGenericServicesConfig
    {
        /// <summary>
        /// This holds the code that will match the Name and Type to the Name/Type of a PropertyInfo
        /// The DefaultNameMatcher only handles names that are exactly the same, apart from the given name can be camelCase
        /// </summary>
        MatchNameAndType NameMatcher { get; }

        /// <summary>
        /// By default the method ReadSingle/Async returns an error if the item asked for is null.
        /// But in Web API this isn't an error, but is handled by returning NotFound status.
        /// Setting this property to true will stop an error being reported on ReadSingle returning null
        /// </summary>
        bool NoErrorOnReadSingleNull { get; }

        /// <summary>
        /// If this is set to true then all Create/Update/Delete done via a direct access to an entity
        /// </summary>
        bool DirectAccessValidateOnSave { get; }

        /// <summary>
        /// If this is set to true then all Create/Update/Delete done via a DTO will be validated
        /// </summary>
        bool DtoAccessValidateOnSave { get; }

        /// <summary>
        /// When SaveChangesWithValidation is called if there is an exception then this method
        /// is called. If it returns null then the error is rethrown, but if it returns a ValidationResult
        /// then that is turned into a error message that is shown to the user via the IBizActionStatus
        /// See section 10.7.3 of my book "Entity Framework Core in Action" on how to use this to turn
        /// SQL errors into user-friendly errors
        /// </summary>
        Func<Exception, DbContext, IStatusGeneric> SaveChangesExceptionHandler { get; }

        /// <summary>
        /// This will be called just before SaveChanges/SaveChangesAsync is called.
        /// I allows you to do things like your own validation, logging, etc. without needing to put code inside your application's DbContext
        /// If the status returned by BeforeSaveChanges has errors, then SaveChanges won't be called. 
        /// </summary>
        BeforeSaveChanges BeforeSaveChanges { get; }
    }
}