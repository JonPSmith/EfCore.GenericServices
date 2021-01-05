// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class BookListNestedV2Dto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public ICollection<AuthorNestedV2Dto> AuthorsLink { get; set; }
    }
}