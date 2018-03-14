// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class AuthorNameDto : ILinkToEntity<Author>
    {
        public string Name { get; set; }
    }
}