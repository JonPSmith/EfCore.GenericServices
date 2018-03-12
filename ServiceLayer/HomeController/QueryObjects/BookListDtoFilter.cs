// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController.QueryObjects
{
    public enum BooksFilterBy
    {
        [Display(Name = "All")]
        NoFilter = 0,
        [Display(Name = "By Votes...")]
        ByVotes,
        [Display(Name = "By Year published...")]
        ByPublicationYear
    }

    public static class BookListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookListDto> FilterBooksBy(
            this IQueryable<BookListDto> books, 
            BooksFilterBy filterBy, string filterValue)         
        {
            if (string.IsNullOrEmpty(filterValue))              
                return books;                                   

            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:                    
                    return books;                               
                case BooksFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);     
                    return books.Where(x =>                      
                          x.ReviewsAverageVotes > filterVote);   
                case BooksFilterBy.ByPublicationYear:             
                    if (filterValue == AllBooksNotPublishedString)
                        return books.Where(                       
                            x => x.PublishedOn > DateTime.UtcNow);

                    var filterYear = int.Parse(filterValue);      
                    return books.Where(                           
                        x => x.PublishedOn.Year == filterYear     
                          && x.PublishedOn <= DateTime.UtcNow);   
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }

        /***************************************************************
        #A The method is given both the type of filter and the user selected filter value
        #B If the filter value isn't set then it returns the IQueryable with no change
        #C Same for no filter selected - it returns the IQueryable with no change
        #D The filter by votes is a value and above, e.g. 3 and above. Note: not reviews returns null, and the test is always false
        #E If the "coming soon" was picked then we only return books not yet published
        #F If we have a specific year we filter on that. Note that we also remove future books (in case the user chose this year's date)
         * ************************************************************/
    }
}