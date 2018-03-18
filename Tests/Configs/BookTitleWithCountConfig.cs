// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using AutoMapper;
using DataLayer.EfClasses;
using GenericServices.Configuration;
using Tests.Dtos;

namespace Tests.Configs
{
    public class BookTitleWithCountConfig : DtoConfigInfo<BookTitleAndCount, Book>
    {
        public override Action<IMappingExpression<Book, BookTitleAndCount>> AlterReadMapping
        {
            get { return cfg => cfg.ForMember(x => x.ReviewsCount, x => x.MapFrom(book => book.Reviews.Count())); }
        }
    }
}