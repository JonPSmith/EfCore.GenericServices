// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
        private readonly ITestOutputHelper _output;

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
            var includeStrings = typeof(AddNewAuthorToBookUsingIncludesDto)
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

                //ATTEMPT
                var query = ApplyAnyIncludeStringsAtDbSetLevel<AddReviewWithIncludeDto>(context.Books);
                var book = query.Single(x => x.Reviews.Any());

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

                var includeStrings = typeof(AddNewAuthorToBookUsingIncludesDto)
                    .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                    .Select(x => x.IncludeNames).ToList();

                //ATTEMPT
                var query = ApplyAnyIncludeStringsAtDbSetLevel<AddReviewWithIncludeDto>(context.Books);
                var books = query.ToList();

                //VERIFY
                var names = books.SelectMany(x => x.AuthorsLink.Select(y => y.Author.Name)).ToArray();
                names.ShouldEqual(new string[] { "Martin Fowler", "Martin Fowler", "Eric Evans", "Future Person" });
            }
        }

        [Fact]
        public void TestIncludeThenTwiceGetAttributesOk()
        {
            //SETUP

            //ATTEMPT
            var includeStrings = typeof(AnotherDto)
                .GetCustomAttributes(typeof(IncludeThenAttribute), true).Cast<IncludeThenAttribute>()
                .Select(x => x.IncludeNames).ToList();

            //VERIFY
            includeStrings.Count.ShouldEqual(2);
            includeStrings.First().ShouldEqual("Reviews");
            includeStrings.Last().ShouldEqual("AuthorsLink.Author");
        }

        [Fact]
        public void TestIncludeThenTwiceGetAttributesManuallyLoad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var query = ApplyAnyIncludeStringsAtDbSetLevel<AnotherDto>(context.Books);
                var book = query.First();

                //VERIFY
                book.Reviews.ShouldNotBeNull();
                book.AuthorsLink.ShouldNotBeNull();
                book.AuthorsLink.Any(x => x.Author == null).ShouldBeFalse();
            }
        }

        //NOTE: This is a copy of the code in the CreateMapper class
        private IQueryable<Book> ApplyAnyIncludeStringsAtDbSetLevel<TDto>(DbSet<Book> dbSet)
        {
            var attributes = typeof(TDto).GetCustomAttributes(typeof(IncludeThenAttribute), true)
                .Cast<IncludeThenAttribute>().ToList();

            if (!attributes.Any())
                return dbSet;

            var query = dbSet.Include(attributes[0].IncludeNames);
            for (int i = 1; i < attributes.Count; i++)
            {
                query = query.Include(attributes[i].IncludeNames);
            }

            return query;
        }

        [IncludeThen(nameof(Book.Reviews))]
        [IncludeThen(nameof(Book.AuthorsLink), nameof(BookAuthor.Author))]
        private class AnotherDto : ILinkToEntity<Book>
        {}
    }
}