// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestIncludeThenAttribute
    {

        [Fact]
        public void TestSingleIncludeThenWithSingleIncludeGetAttributesOk()
        {
            //SETUP

            //ATTEMPT
            var attributes = typeof(AddReviewWithIncludeDto).GetCustomAttributes(false).Cast<IncludeThenAttribute>().ToList();

            //VERIFY
            attributes.Count.ShouldEqual(1);
            attributes.First().IncludeNames.ShouldEqual(nameof(Book.Reviews));
        }

        [Fact]
        public void TestSingleIncludeThenWithCombinedIncludeGetAttributesOk()
        {
            //SETUP

            //ATTEMPT
            var attributes = typeof(UpdateBookWithAuthorUsingIncludeDto).GetCustomAttributes(false).Cast<IncludeThenAttribute>().ToList();

            //VERIFY
            attributes.Count.ShouldEqual(1);
            attributes.First().IncludeNames.ShouldEqual($"{nameof(Book.AuthorsLink)}.{nameof(BookAuthor.Author)}");
        }

        [Fact]
        public void TestIncludeThenOneLevelManuallyLoad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var attributes = typeof(AddReviewWithIncludeDto).GetCustomAttributes(false).Cast<IncludeThenAttribute>().ToList();

                //ATTEMPT
                var book = context.Books.Include(attributes.First().IncludeNames)
                    .Single(x => x.Reviews.Any());

                //VERIFY
                book.Reviews.Any().ShouldBeTrue();
            }
        }

        [Fact]
        public void TestIncludeThenTwoLevelManuallyLoad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var attributes = typeof(UpdateBookWithAuthorUsingIncludeDto).GetCustomAttributes(false).Cast<IncludeThenAttribute>().ToList();

                //ATTEMPT
                var books = context.Books.Include(attributes.First().IncludeNames).ToList();

                //VERIFY
                var names = books.SelectMany(x => x.AuthorsLink.Select(y => y.Author.Name)).ToArray();
                names.ShouldEqual(new String []{ "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
            }
        }

    }
}