// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestReadViaDto
    {
        //------------------------------------------------------
        //ReadSingle

        [Fact]
        public void TestProjectBookTitleSingleOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var dto = service.ReadSingle<BookTitle>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestProjectSingleOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var dto = service.ReadSingle<BookTitleAndCount>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestProjectSingleWhereOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var dto = service.ReadSingle<BookTitleAndCount>(x => x.BookId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestProjectSingleBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var dto = service.ReadSingle<BookTitleAndCount>(999);

                //VERIFY
                service.IsValid.ShouldBeFalse(service.GetAllErrors());
                service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book Title And Count you were looking for.");
            }
        }

        [Fact]
        public void TestProjectSingleWhereBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => service.ReadSingle<BookTitleAndCount>(x => true));

                //VERIFY
                ex.Message.ShouldEqual("Sequence contains more than one element");
            }
        }

        //------------------------------------------------------
        //ReadSingleToDto

        [Fact]
        public void TestReadSingleToDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);
                var dto = new BookTitle {BookId = -1, Title = "Original title"};

                //ATTEMPT
                service.ReadSingleToDto(dto, 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestReadSingleToDtoKeysInDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);
                var dto = new BookTitle { BookId = 1, Title = "Original title" };

                //ATTEMPT
                service.ReadSingleToDto(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestReadSingleToDtoBadKey()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);
                var dto = new BookTitle { BookId = -1, Title = "Original title" };

                //ATTEMPT
                service.ReadSingleToDto(dto, -1);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book Title you were looking for.");
            }
        }

        [Fact]
        public void TestReadSingleToDtoBadDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);
                var dto = "";

                //ATTEMPT
                var ex = Assert.Throws<NullReferenceException>(() => service.ReadSingleToDto(dto, -1));

                //VERIFY
                ex.Message.ShouldEqual("The class System.String is not registered as a valid GenericService DTO/ViewModel. Have you left off the ILinkToEntity interface?");
            }
        }

        [Fact]
        public void TestReadSingleToDtoWhereOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);
                var dto = new BookTitle { BookId = -1, Title = "Original title" };

                //ATTEMPT
                service.ReadSingleToDto(dto, x => x.BookId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }


        //------------------------------------------------------
        //Read many

        [Fact]
        public void TestProjectBookTitleManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var list = service.ReadManyNoTracked<BookTitle>().ToList();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                list.Count.ShouldEqual(4);
                list.Select(x => x.Title).ShouldEqual(new []{ "Refactoring", "Patterns of Enterprise Application Architecture", "Domain-Driven Design", "Quantum Networking" });
            }
        }

        [Fact]
        public void TestProjectBookTitleManyWithConfigOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var wrapped = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
                var service = new GenericService(context, wrapped);

                //ATTEMPT
                var list = service.ReadManyNoTracked<BookTitleAndCount>().ToList();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                list.Count.ShouldEqual(4);
                list.Select(x => x.Title).ShouldEqual(new[] { "Refactoring", "Patterns of Enterprise Application Architecture", "Domain-Driven Design", "Quantum Networking" });
                list.Select(x => x.ReviewsCount).ShouldEqual(new []{0,0,0,2});
            }
        }



        //[Fact]
        //public void TestUpdateEntityOk()
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
        //        var service = new GenericService(context, mapper);

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