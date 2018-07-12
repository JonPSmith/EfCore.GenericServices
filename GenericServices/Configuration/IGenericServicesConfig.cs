// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Configuration
{
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
        /// If this is set to true then all Create/Update/Delete done via a direct access to an entity
        /// </summary>
        bool DirectAccessValidateOnSave { get; }

        /// <summary>
        /// If this is set to true then all Create/Update/Delete done via a DTO will be validated
        /// </summary>
        bool DtoAccessValidateOnSave { get; }

        /// <summary>
        /// When SaveChangesWithValidation is called if there is a DbUpdateException then this method
        /// is called. If it returns null then the error is rethrown, but if it returns a ValidationResult
        /// then that is turned into a error message that is shown to the user via the IBizActionStatus
        /// See section 10.7.3 of my book "Entity Framework Core in Action" on how to use this to turn
        /// SQL errors into user-friendly errors
        /// </summary>
        Func<DbUpdateException, ValidationResult> SqlErrorHandler { get; }
    }
}