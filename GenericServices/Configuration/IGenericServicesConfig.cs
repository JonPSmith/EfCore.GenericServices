// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace GenericServices.Configuration
{
    [Flags]
    public enum CrudTypes
    {
        None = 0,
        Create = 1,
        Read = 2,
        Update = 4,
        Delete = 8,
    }

    public interface IGenericServicesConfig
    {
        /// <summary>
        /// This holds the code that will match the Name and Type to the Name/Type of a PropertyInfo
        /// The DefaultNameMatcher only handles names that are exactly the same, apart from the given name can be camelCase
        /// </summary>
        MatchNameAndType NameMatcher { get; }

        /// <summary>
        /// By default the properties in the DTO/VM call that are null or have a [ReadOnly(true)] attribute will NOT be
        /// copied back to the entity class. This applies when you do a create or update that uses AutoMapper.
        /// You can set this to true to turn off that feature.
        /// NOTE: This flag does NOT affect the use of the [ReadOnly(true)] attribute in DDD access methods  
        /// </summary>
        bool TurnOffAuthoMapperSaveFilter { get; }
    }
}