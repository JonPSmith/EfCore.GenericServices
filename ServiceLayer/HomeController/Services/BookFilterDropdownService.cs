// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.HomeController.QueryObjects;

namespace ServiceLayer.HomeController.Services
{
    public class BookFilterDropdownService
    {
        private readonly EfCoreContext _db;

        public BookFilterDropdownService(EfCoreContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public IEnumerable<DropdownTuple> GetFilterDropDownValues(BooksFilterBy filterBy)
        {
            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:
                    //return an empty list
                    return new List<DropdownTuple>();
                case BooksFilterBy.ByVotes:
                    return FormVotesDropDown();
                case BooksFilterBy.ByPublicationYear:
                    var comingSoon = _db.Books.                     
                        Any(x => x.PublishedOn > DateTime.UtcNow);  
                    var nextYear = DateTime.UtcNow.AddYears(1).Year;
                    var result = _db.Books                          
                        .Select(x => x.PublishedOn.Year)            
                        .Distinct()                                 
                        .Where(x => x < nextYear)                   
                        .OrderByDescending(x => x)                  
                        .Select(x => new DropdownTuple              
                        {                                           
                            Value = x.ToString(),                   
                            Text = x.ToString()                     
                        }).ToList();                                
                    if (comingSoon)                                 
                        result.Insert(0, new DropdownTuple          
                        {
                            Value = BookListDtoFilter.AllBooksNotPublishedString,
                            Text = BookListDtoFilter.AllBooksNotPublishedString
                        });

                    return result;
                /*****************************************************************
                #A This returns true if there is a book in the list that is not yet published
                #B This gets next year so we can filter out all future publications
                #C This long command gets the year of publication, uses distinct to only have one of each year, filters out the future books and orders with newest year at the top
                #D I finally use two client/server evaluations to turn the values into strings
                #E Finally I add a "coming soon" filter for all the future books
                 * ***************************************************************/
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
            }
        }

        //------------------------------------------------------------
        // private methods

        private static IEnumerable<DropdownTuple> FormVotesDropDown()
        {
            return new[]
            {
                new DropdownTuple {Value = "4", Text = "4 stars and up"},
                new DropdownTuple {Value = "3", Text = "3 stars and up"},
                new DropdownTuple {Value = "2", Text = "2 stars and up"},
                new DropdownTuple {Value = "1", Text = "1 star and up"},
            };
        }
    }
}