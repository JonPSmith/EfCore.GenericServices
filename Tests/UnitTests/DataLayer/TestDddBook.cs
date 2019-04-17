// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.DataLayer
{
    public class TestDddBook
    {

        [Fact]
        public void TestCreateBookOk()
        {
            //SETUP

            //ATTEMPT
            var status = Book.CreateBook(
                "Book Title",
                "Book Description",
                new DateTime(2000, 1, 1),
                "Book Publisher",
                123,
                null,
                new[] {new Author {Name = "Test Author"}}
                );

            //VERIFY
            status.IsValid.ShouldBeTrue(status.GetAllErrors());   
        }

        [Fact]
        public void TestCreateBookNoTitleBad()
        {
            //SETUP

            //ATTEMPT
            var status = Book.CreateBook(
                "",
                "Book Description",
                new DateTime(2000, 1, 1),
                "Book Publisher",
                123,
                null,
                new[] { new Author { Name = "Test Author" } }
            );

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.GetAllErrors().ShouldEqual("The book title cannot be empty.");
        }

        [Fact]
        public void TestCreateBookNoAuthorsBad()
        {
            //SETUP

            //ATTEMPT
            var status = Book.CreateBook(
                "Book Title",
                "Book Description",
                new DateTime(2000, 1, 1),
                "Book Publisher",
                123,
                null,
                new Author[0] 
            );

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.GetAllErrors().ShouldEqual("You must have at least one Author for a book.");
        }

        [Fact]
        public void TestAddReviewToBookWithIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.Include(x => x.Reviews).First();
                book.AddReview(5, "comment", "user");
                context.SaveChanges();

                //VERIFY
                book.Reviews.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(3);
            }
        }


        [Fact]
        public void TestAddReviewToBookNoIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First();
                book.AddReview(5, "comment", "user", context);
                context.SaveChanges();

                //VERIFY
                book.Reviews.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestAddReviewToBookNoIncludeError()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First();
                var ex = Assert.Throws<ArgumentNullException>(() => book.AddReview(5, "comment", "user"));

                //VERIFY
                ex.Message.ShouldStartWith("You must provide a context if the Reviews collection isn't valid.");
            }
        }

        [Fact]
        public void TestRemoveReviewBookWithIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.Include(x => x.Reviews).Single(x => x.Reviews.Count() == 2);
                book.RemoveReview(book.Reviews.LastOrDefault());
                context.SaveChanges();

                //VERIFY
                book.Reviews.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestRemoveReviewBookNoExistingReviewsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.AsNoTracking().Single(x => x.Reviews.Count() == 2);
                book.RemoveReview(context.Set<Review>().First(), context);
                context.SaveChanges();

                //VERIFY
                context.Set<Review>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestRemoveReviewBookNotLinkedToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First(x => !x.Reviews.Any());
                var ex = Assert.Throws<InvalidOperationException>(() => 
                    book.RemoveReview(context.Set<Review>().First(), context));

                //VERIFY
                ex.Message.ShouldEqual("The review either hasn't got a valid primary key or was not linked to the Book.");
            }
        }

        [Fact]
        public void TestAddPromotionBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First();
                var status = book.AddPromotion(book.OrgPrice / 2, "Half price today");
                context.SaveChanges();

                //VERIFY
                status.IsValid.ShouldBeTrue();
                book.ActualPrice.ShouldEqual(book.OrgPrice / 2);
            }
        }

        [Fact]
        public void TestAddPromotionBookWithError()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First();
                var status = book.AddPromotion(book.OrgPrice / 2, "");

                //VERIFY
                status.IsValid.ShouldBeFalse();
                status.Errors.Single().ToString().ShouldEqual("You must provide some text to go with the promotion.");
            }
        }

        [Fact]
        public void TestRemovePromotionBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.OrderByDescending(x => x.BookId).First();
                book.RemovePromotion();
                context.SaveChanges();

                //VERIFY
                book.ActualPrice.ShouldEqual(book.OrgPrice);
            }
        }

    }

}