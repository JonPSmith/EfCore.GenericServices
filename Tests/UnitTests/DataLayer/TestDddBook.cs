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
        public void TestRemoveReviewBookOk()
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
                status.HasErrors.ShouldBeFalse();
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
                status.HasErrors.ShouldBeTrue();
                status.Errors.Single().ErrorMessage.ShouldEqual("You must provide some text to go with the promotion.");
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