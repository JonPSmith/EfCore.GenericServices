// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using AutoMapper;
using DataLayer.EfClasses;
using GenericServices.Configuration;

namespace ServiceLayer.HomeController.Dtos
{
    class BookListDtoConfig : PerDtoConfig<BookListDto, Book>
    {
        public override Action<IMappingExpression<Book, BookListDto>> AlterReadMapping
        {
            get
            {
                return cfg => cfg
                    .ForMember(x => x.ReviewsCount, x => x.MapFrom(book => book.Reviews.Count()))
                    .ForMember(x => x.AuthorsOrdered, y => y.MapFrom(p => string.Join(", ",
                        p.AuthorsLink.OrderBy(q => q.Order).Select(q => q.Author.Name))))
                    .ForMember(x => x.ReviewsAverageVotes,
                        x => x.MapFrom(p => p.Reviews.Select(y => (double?)y.NumStars).Average()));
            }
        }
    }
}