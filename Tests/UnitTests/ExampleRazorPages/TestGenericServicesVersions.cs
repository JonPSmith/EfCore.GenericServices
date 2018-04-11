// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController.Dtos;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using AddReviewDto = ServiceLayer.HomeController.Dtos.AddReviewDto;

namespace Tests.UnitTests.ExampleRazorPages
{
    public class TestGenericServicesVersions
    {
        [Fact]
        public void TestCreateBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<CreateBookDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new CreateBookDto { Title = "Hello", Price = 50, PublishedOn = new DateTime(2010,1,1)};
                dto.BookAuthorIds.Add(1);
                dto.BeforeSave(context);
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.Set<Book>().Count().ShouldEqual(5);
                var book = context.Books.Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .Single(x => x.BookId == dto.BookId);
                book.AuthorsLink.Single().Author.Name.ShouldEqual("Martin Fowler");
            }
        }

        [Fact]
        public void TestCreateBookBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<CreateBookDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new CreateBookDto { Title = "", Price = 50, PublishedOn = new DateTime(2010, 1, 1) };
                dto.BookAuthorIds.Add(1);
                dto.BeforeSave(context);
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("The book title cannot be empty.");
            }
        }

        [Fact]
        public void TestCreateBookUseCtorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<CreatorOfBooksDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new CreatorOfBooksDto
                {
                    Title = "Hello",
                    Price = 50,
                    PublishedOn = new DateTime(2010, 1, 1),
                    Authors = new List<Author> { new Author { Name = "test" } }
                };
                service.CreateAndSave(dto, "ctor");

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.Set<Book>().Count().ShouldEqual(1);
                var book = context.Find<Book>(1);
                book.ActualPrice.ShouldEqual(dto.Price);
            }
        }

        [Fact]
        public void TestCreateBookUseStaticMethodOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<CreatorOfBooksDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new CreatorOfBooksDto
                {
                    Title = "Hello",
                    Price = 50,
                    PublishedOn = new DateTime(2010, 1, 1),
                    Authors = new List<Author> { new Author { Name = "test" } }
                };
                service.CreateAndSave(dto, nameof(Book.CreateBook));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.Set<Book>().Count().ShouldEqual(1);
                var book = context.Find<Book>(1);
                book.ActualPrice.ShouldEqual(dto.Price);
            }
        }

        [Fact]
        public void TestCreateBookBothStaticMethodAndCtorBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<CreatorOfBooksDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new CreatorOfBooksDto();
                var ex = Assert.Throws<InvalidOperationException>(() => service.CreateAndSave(dto));

                //VERIFY
                ex.Message.ShouldStartWith("There are multiple ctor/static method,");
            }
        }


        [Fact]
        public void TestAddReviewToBookOk()
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
                var utData = context.SetupSingleDtoAndEntities<AddReviewDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new AddReviewDto {BookId = 1, NumStars = 2};
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
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
            }

            using (var context = new EfCoreContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<AddRemovePromotionDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new AddRemovePromotionDto {BookId = 1, ActualPrice = 20, PromotionalText = "Half price today!"};
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.IsValid.ShouldBeTrue();
                var book = context.Find<Book>(1);
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
                var utData = context.SetupSingleDtoAndEntities<AddRemovePromotionDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var dto = new AddRemovePromotionDto { BookId = 1, PromotionalText = "" };
                service.UpdateAndSave(dto);

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
            }
            using (var context = new EfCoreContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<AddRemovePromotionDto>();
                var service = new CrudServices(context, utData.Wrapped);

                var dto = new AddRemovePromotionDto { BookId = 1, ActualPrice = 20, PromotionalText = "Half price today!" };
                service.UpdateAndSave(dto);

                var book1 = context.Find<Book>(1);

                //ATTEMPT
                service.UpdateAndSave(dto, nameof(Book.RemovePromotion));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                var book = context.Find<Book>(1);
                book.ActualPrice.ShouldEqual(40);
            }
        }

    }

}