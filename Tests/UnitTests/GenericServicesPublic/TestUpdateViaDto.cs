// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
using Microsoft.AspNetCore.Mvc;
using Tests.Dtos;
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

                var wrapped = context.SetupSingleDtoAndEntities<AuthorDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var dto = new AuthorDto { AuthorId = 1, Name = "New Name", Email = "you@gmail.com" };
                service.Update(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
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

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000,1,1) };
                service.Update(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
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

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                service.Update(dto, nameof(Book.RemovePromotion));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                var entity = context.Books.Find(4);
                entity.ActualPrice.ShouldEqual(220);
            }
        }

        public class ConfigSettingMethod : PerDtoConfig<DtoWithConfig, Book>
        {
            public override string UpdateMethod { get; } = nameof(Book.RemovePromotion);
        }

        public class DtoWithConfig : ILinkToEntity<Book>, IConfigFoundIn<ConfigSettingMethod>
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

                var wrapped = context.SetupSingleDtoAndEntities<DtoWithConfig>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var dto = new DtoWithConfig { BookId = 4 };
                service.Update(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
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

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                var ex = Assert.Throws<InvalidOperationException>(() => service.Update(dto, nameof(Book.AddReview)));

                //VERIFY
                ex.Message.ShouldStartWith("Could not find a method of name AddReview. The method that fit the properties in the DTO/VM are:");
            }
        }

        [Fact]
        public void TestUpdateViaAutoMapperBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                var ex = Assert.Throws<InvalidOperationException>(() => service.Update(dto, "AutoMapper"));

                //VERIFY
                ex.Message.ShouldStartWith("There was no way to update the entity class Book using AutoMapper.");
            }
        }

    }
}