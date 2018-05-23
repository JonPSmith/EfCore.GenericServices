// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class BookListNestedV1Dto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public ICollection<AuthorNestedV1Dto> AuthorsLink { get; set; }
    }
}