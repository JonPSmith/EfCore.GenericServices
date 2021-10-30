// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController.Services;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.ExampleRazorPages
{
    public class TestHandCodedVersions
    {
        [Fact]
        public void TestAddReviewToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                context.ChangeTracker.Clear();

                var service = new AddReviewService(context);

                //ATTEMPT
                var dto = service.GetOriginal(1);
                dto.NumStars = 2;
                service.AddReviewToBook(dto);
                context.SaveChanges();

                //VERIFY
                context.Set<Review>().Count().ShouldEqual(3);
                context.Books.Include(x => x.Reviews).Single(x => x.BookId == 1).Reviews.Single().NumStars.ShouldEqual(2);
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

                context.ChangeTracker.Clear();

                var service = new AddRemovePromotionService(context);

                //ATTEMPT
                var dto = service.GetOriginal(1);
                dto.ActualPrice = dto.OrgPrice / 2;
                dto.PromotionalText = "Half price today!";
                var book = service.AddPromotion(dto);
                context.SaveChanges();

                //VERIFY
                service.IsValid.ShouldBeTrue();
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

                context.ChangeTracker.Clear();

                var service = new AddRemovePromotionService(context);

                //ATTEMPT
                var dto = service.GetOriginal(1);
                dto.ActualPrice = dto.OrgPrice / 2;
                dto.PromotionalText = "";
                service.AddPromotion(dto);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.Single().ToString().ShouldEqual("You must provide some text to go with the promotion.");
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

                context.ChangeTracker.Clear();

                var service = new AddRemovePromotionService(context);
                var dto = service.GetOriginal(1);
                dto.ActualPrice = dto.OrgPrice / 2;
                dto.PromotionalText = "";
                service.AddPromotion(dto);

                //ATTEMPT
                var book = service.RemovePromotion(dto.BookId);

                //VERIFY
                book.ActualPrice.ShouldEqual(dto.OrgPrice);
            }
        }
    }

}