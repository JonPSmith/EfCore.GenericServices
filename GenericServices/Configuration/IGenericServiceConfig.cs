// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices.Configuration
{
    public interface IGenericServiceConfig
    {
        /// <summary>
        /// This holds the code that will match the Name and Type to the Name/Type of a PropertyInfo
        /// The DefaultNameMatcher only handles names that are exactly the same, apart from the given name can be camelCase
        /// </summary>
        MatchNameAndType NameMatcher { get; }
    }
}