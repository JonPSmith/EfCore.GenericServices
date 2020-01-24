// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestIncludeThenAttribute
    {
        private ITestOutputHelper _output;

        public TestIncludeThenAttribute(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSingleIncludeThenWithSingleIncludeGetAttributesOk()
        {
            //SETUP

            //ATTEMPT
            var includeStrings = typeof(AddReviewWithIncludeDto)
                .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                .Select(x => x.IncludeNames).ToList();

            //VERIFY
            includeStrings.Count.ShouldEqual(1);
            includeStrings.First().ShouldEqual(nameof(Book.Reviews));
        }

        [Fact]
        public void TestSingleIncludeThenWithSingleIncludeGetAttributesPerformance()
        {
            //SETUP

            //ATTEMPT
            using(new TimeThings(_output, "read one attribute"))
            {
                var includeStrings = typeof(AddReviewWithIncludeDto)
                    .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                    .Select(x => x.IncludeNames).ToList();
            }

            const int numTimes = 100;
            using (new TimeThings(_output, "read one attribute", numTimes))
            {
                for (int i = 0; i < numTimes; i++)
                {
                    var includeStrings = typeof(AddReviewWithIncludeDto)
                        .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                        .Select(x => x.IncludeNames).ToList();
                }
            }

            //VERIFY

        }

        [Fact]
        public void TestSingleIncludeThenWithCombinedIncludeGetAttributesOk()
        {
            //SETUP

            //ATTEMPT
            var includeStrings = typeof(UpdateBookWithAuthorUsingIncludeDto)
                .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                .Select(x => x.IncludeNames).ToList();

            //VERIFY
            includeStrings.Count.ShouldEqual(1);
            includeStrings.First().ShouldEqual($"{nameof(Book.AuthorsLink)}.{nameof(BookAuthor.Author)}");
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

                var includeStrings = typeof(AddReviewWithIncludeDto)
                    .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                    .Select(x => x.IncludeNames).ToList();

                //ATTEMPT
                var book = context.Books.Include(includeStrings.First())
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

                var includeStrings = typeof(UpdateBookWithAuthorUsingIncludeDto)
                    .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                    .Select(x => x.IncludeNames).ToList();

                //ATTEMPT
                var books = context.Books.Include(includeStrings.First()).ToList();

                //VERIFY
                var names = books.SelectMany(x => x.AuthorsLink.Select(y => y.Author.Name)).ToArray();
                names.ShouldEqual(new String[] { "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
            }
        }

    }
}