// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace GenericServices.Configuration
{
    public delegate PropertyMatch MatchNameAndType(string name, Type type, PropertyInfo propertyInfo);

    public class GenericServicesConfig : IGenericServicesConfig
    {
       
        /// <summary>
        /// This holds the code that will match the Name and Type to the Name/Type of a PropertyInfo
        /// The DefaultNameMatcher only handles names that are exactly the same, apart from the given name can be camelCase
        /// </summary>
        public MatchNameAndType NameMatcher { get; set; } = DefaultNameMatcher.MatchCamelAndPascalName;

        /// <summary>
        /// By default the properties in the DTO/VM call that are null or have a [ReadOnly(true)] attribute will NOT be
        /// copied back to the entity class. This applies when you do a create or update that uses AutoMapper.
        /// You can set this to true to turn off that feature.
        /// NOTE: This flag does NOT affect the use of the [ReadOnly(true)] attribute in DDD access methods  
        /// </summary>
        public bool TurnOffAuthoMapperSaveFilter { get; set; } = false;

        /// <summary>
        /// This allows you to make all CRUD SaveChanges to call the extention method SaveChangesWithValidation
        /// It is unlikely you will want that, as your front-end should validate data. 
        /// You can turn on validation on a per-DTO basis using the PerDtoConfig and the IConfigFoundIn interface
        /// </summary>
        public bool CrudSaveUseValidation { get; set; } = false;

    }
}