// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace GenericServices.Configuration
{
    public interface IGenericServicesConfig
    {
        /// <summary>
        /// This holds the code that will match the Name and Type to the Name/Type of a PropertyInfo
        /// The DefaultNameMatcher only handles names that are exactly the same, apart from the given name can be camelCase
        /// </summary>
        MatchNameAndType NameMatcher { get; }

        /// <summary>
        /// This allows you to make all CRUD SaveChanges to call the extention method SaveChangesWithValidation
        /// It is unlikely you will want that, as your front-end should validate data. 
        /// You can turn on validation on a per-DTO basis using the PerDtoConfig and the IConfigFoundIn interface
        /// </summary>
        bool CrudSaveUseValidation { get; }
    }
}