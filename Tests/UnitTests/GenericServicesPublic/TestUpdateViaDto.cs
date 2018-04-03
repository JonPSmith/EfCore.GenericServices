// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
using ServiceLayer.HomeController.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestUpdateViaDto
    {
        public class AuthorDto : ILinkToEntity<Author>
        {
            public int AuthorId { get; set; }
            [ReadOnly(true)]
            public string Name { get; set; }
            public string Email { get; set; }
        }

        [Fact]
        public void TestUpdateViaAutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new Author {Name = "Start Name", Email = "me@nospam.com"});
                context.SaveChanges();

                var wrapped = context.SetupSingleDtoAndEntities<AuthorDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new AuthorDto { AuthorId = 1, Name = "New Name", Email = "you@gmail.com" };
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Author");
                var entity = context.Authors.Find(1);
                entity.Name.ShouldEqual("Start Name");
                entity.Email.ShouldEqual(dto.Email);
            }
        }

        [Fact]
        public void TestUpdateViaDefaultMethodOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000,1,1) };
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Find(4);
                entity.PublishedOn.ShouldEqual(new DateTime(2000, 1, 1));
            }
        }

        [Fact]
        public void TestUpdatePublicationDateViaAutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                service.UpdateAndSave(dto, "AutoMapper");

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Find(4);
                entity.PublishedOn.ShouldEqual(new DateTime(2000, 1, 1));
            }
        }

        [Fact]
        public void TestUpdateViaStatedMethodOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                service.UpdateAndSave(dto, nameof(Book.RemovePromotion));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Find(4);
                entity.ActualPrice.ShouldEqual(220);
            }
        }

        [Fact]
        public void TestUpdateAddReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.AddReviewDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.AddReviewDto {BookId = 1, Comment = "comment", NumStars = 3, VoterName = "user" };
                service.UpdateAndSave(dto, nameof(Book.AddReview));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                context.Set<Review>().Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestUpdateAddPromotionWithMessageOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var wrapped = context.SetupSingleDtoAndEntities<AddRemovePromotionDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new AddRemovePromotionDto { BookId = 1, ActualPrice = 1, PromotionalText = "Really Cheap!"};
                service.UpdateAndSave(dto, nameof(Book.AddPromotion));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("The book's new price is $1.00.");
            }
        }

        public class ConfigSettingMethod : PerDtoConfig<DtoWithConfig, Book>
        {
            public override string UpdateMethod { get; } = nameof(Book.RemovePromotion);
        }

        public class DtoWithConfig : ILinkToEntity<Book>
        {
            public int BookId { get; set; }
            public string Title { get; set; }
        }

        [Fact]
        public void TestUpdateViaStatedMethodInPerDtoConfigOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<DtoWithConfig>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new DtoWithConfig { BookId = 4 };
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Find(4);
                entity.ActualPrice.ShouldEqual(220);
            }
        }

        [Fact]
        public void TestUpdateViaStatedMethodBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateAndSave(dto, nameof(Book.AddReview)));

                //VERIFY
                ex.Message.ShouldStartWith("Could not find a method of name AddReview. The method that fit the properties in the DTO/VM are:");
            }
        }
    }
}