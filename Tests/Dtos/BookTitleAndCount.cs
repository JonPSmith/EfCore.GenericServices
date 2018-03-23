// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using DataLayer.EfClasses;
using GenericServices;
using GenericServices.Configuration;
using Tests.Configs;

namespace Tests.Dtos
{
    public class BookTitleAndCount : ILinkToEntity<Book>, IConfigFoundIn<BookTitleAndCountConfig>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int ReviewsCount { get; set; }

        public static MapperConfiguration Config => new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Book, BookTitleAndCount>()
                .ForMember(x => x.ReviewsCount, x => x.MapFrom(book => book.Reviews.Count()));
        });
    }
}