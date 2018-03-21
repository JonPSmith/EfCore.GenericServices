// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
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
        public void TestUpdateAuthorViaAutoMapperOk()
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
        public void TestUpdateAuthorViaDefaultMethodOk()
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

    }
}