// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class BookTitle : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
    }
}