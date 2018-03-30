// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using GenericServices;

namespace ServiceLayer.HomeController.Dtos
{
    public class BookListDto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public DateTime PublishedOn { get; set; }
        public decimal OrgPrice { get; set; }
        public decimal ActualPrice { get; set; }
        public string PromotionalText { get; set; }
        public string AuthorsOrdered { get; set; }

        public int ReviewsCount { get; set; }
        public double? ReviewsAverageVotes { get; set; }
    }
}