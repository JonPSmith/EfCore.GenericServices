// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License file in the project root for license information.

using System;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices
{
    /// <summary>
    /// Add this attribute to your DTO/ViewModel that has the <see cref="ILinkToEntity{T}"/> applied to it.
    /// You can add multiple IncludeThen attributes to a DTO/ViewModel
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IncludeThenAttribute : Attribute
    {
        public string IncludeNames { get; private set; }

        /// <summary>
        /// This will cause a Include to be added to the database query in an UpdateAndSave call
        /// Useful when updating something in the main entity class where you need a relationships, e.g. adding a new Review.
        /// NOTE: This only works on the DTO that is used in the GenericServices methods calls. IncludeThen attributes on Nested DTOs are ignored. 
        /// </summary>
        /// <param name="includeName">The names of the relationships you want to include. Can have multiple names, e.g. "AuthorLink.Author"
        /// but we recommend using nameof(EntityType.RelationalPropertyName), e.g. nameof(Book.Reviews)</param>
        /// <param name="thenIncludeNames">Optional: If using nameof approach then you can provide a series of ThenInclude names</param>
        public IncludeThenAttribute(string includeName, params string[] thenIncludeNames)
        {
            IncludeNames = includeName;
            if (thenIncludeNames.Any())
                IncludeNames += "." + string.Join(".", thenIncludeNames);
        }
    }
}