// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class AuthorNestedV2Dto : ILinkToEntity<BookAuthor>
    {
        public byte Order { get; set; }
        public string AStringToHoldAuthorName { get; set; }
    }
}