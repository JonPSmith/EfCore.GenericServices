// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestReadViaDtoAsync
    {
        //------------------------------------------------------
        //ReadSingle

        [Fact]
        public async Task TestProjectBookTitleSingleOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = await service.ReadSingleAsync<BookTitle>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public async Task TestProjectSingleOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = await service.ReadSingleAsync<BookTitleAndCount>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public async Task TestProjectSingleWhereOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = await service.ReadSingleAsync<BookTitleAndCount>(x => x.BookId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public async Task TestProjectSingleBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = await service.ReadSingleAsync<BookTitleAndCount>(999);

                //VERIFY
                service.IsValid.ShouldBeFalse(service.GetAllErrors());
                service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book you were looking for.");
            }
        }

        [Fact]
        public async Task TestProjectSingleWhereBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReadSingleAsync<BookTitleAndCount>(x => true));

                //VERIFY
#if NETCOREAPP2_1
                ex.Message.ShouldEqual("Source sequence contains more than one element.");
#elif NETCOREAPP3_0
                ex.Message.ShouldEqual("Enumerator failed to MoveNextAsync.");
#endif
            }
        }

 

        //-------------------------------------------------------
        //Read Single - including collection

        [Fact]
        public async Task TestReadSingleCollection()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookWithAuthors>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = await service.ReadSingleAsync<BookWithAuthors>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Authors.Count.ShouldEqual(1);
            }
        }

        //------------------------------------------------------
        //Read many

        [Fact]
        public async Task TestProjectBookTitleManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var list = await service.ReadManyNoTracked<BookTitle>().ToListAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                list.Count.ShouldEqual(4);
                list.Select(x => x.Title).ShouldEqual(new []{ "Refactoring", "Patterns of Enterprise Application Architecture", "Domain-Driven Design", "Quantum Networking" });
            }
        }

        [Fact]
        public async Task TestProjectBookTitleManyWithConfigOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var list = await service.ReadManyNoTracked<BookTitleAndCount>().ToListAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                list.Count.ShouldEqual(4);
                list.Select(x => x.Title).ShouldEqual(new[] { "Refactoring", "Patterns of Enterprise Application Architecture", "Domain-Driven Design", "Quantum Networking" });
                list.Select(x => x.ReviewsCount).ShouldEqual(new []{0,0,0,2});
            }
        }



        //[Fact]
        //public async Task TestUpdateEntityOk()
        //{
        //    //SETUP
        //    var mapper = AutoMapperHelpers.CreateSaveConfig<AuthorNameDto, Author>();
        //    var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        //    using (var context = new EfCoreContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //        context.SeedDatabaseFourBooks();
        //    }
        //    using (var context = new EfCoreContext(options))
        //    {
        //        var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

        //        //ATTEMPT
        //        var dto = new AuthorNameDto { Name = "New Name" };
        //        service.UpdateAndSave(dto);

        //        //VERIFY
        //        service.IsValid.ShouldBeTrue(service.GetAllErrors());
        //    }
        //    using (var context = new EfCoreContext(options))
        //    {
        //        context.Authors.Count().ShouldEqual(1);
        //        context.Authors.Find(1).Name.ShouldEqual("New Name");
        //    }
        //}
    }
}