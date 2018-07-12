// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestNestedDtos
    {
        [Fact]
        public void TestNestedDtosV1Ok()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();

                var utData = context.SetupSingleDtoAndEntities<BookListNestedV1Dto>();
                utData.AddSingleDto<AuthorNestedV1Dto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = service.ReadSingle<BookListNestedV1Dto>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.AuthorsLink.Count.ShouldEqual(2);
                dto.AuthorsLink.First().AuthorName.ShouldStartWith("Author");
                dto.AuthorsLink.First().Order.ShouldEqual((byte)0);
                dto.AuthorsLink.Last().AuthorName.ShouldEqual("CommonAuthor");
                dto.AuthorsLink.Last().Order.ShouldEqual((byte)1);
            }
        }

        [Fact]
        public void TestNestedDtosV2Ok()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks();

                var utData = context.SetupSingleDtoAndEntities<BookListNestedV2Dto>();
                utData.AddSingleDto<AuthorNestedV2Dto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = service.ReadSingle<BookListNestedV2Dto>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.AuthorsLink.Count.ShouldEqual(2);
                dto.AuthorsLink.First().AStringToHoldAuthorName.ShouldStartWith("Author");
                dto.AuthorsLink.First().Order.ShouldEqual((byte)0);
                dto.AuthorsLink.Last().AStringToHoldAuthorName.ShouldEqual("CommonAuthor");
                dto.AuthorsLink.Last().Order.ShouldEqual((byte)1);
            }
        }
    }
}