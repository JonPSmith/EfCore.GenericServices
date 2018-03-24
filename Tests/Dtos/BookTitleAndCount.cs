// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class BookTitleAndCount : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int ReviewsCount { get; set; }
    }
}