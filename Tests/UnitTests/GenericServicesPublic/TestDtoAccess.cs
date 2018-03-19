// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
    public class TestDtoAccess
    {
        [Fact]
        public void TestProjectBookTitleSingleOk()
        {
            //SETUP
            var mapper = AutoMapperHelpers.CreateWrapperMapper<Book, BookTitle>();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var dto = service.GetSingle<BookTitle>(1);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestProjectSingleOk()
        {
            //SETUP
            var mapper = new WrappedAutoMapperConfig ( BookTitleAndCount.Config);
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var dto = service.GetSingle<BookTitleAndCount>(1);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public void TestProjectBookTitleManyOk()
        {
            //SETUP
            var mapper = AutoMapperHelpers.CreateWrapperMapper<Book, BookTitle>();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var list = service.GetManyNoTracked<BookTitle>().ToList();

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
                list.Count.ShouldEqual(4);
                list.Select(x => x.Title).ShouldEqual(new []{ "Refactoring", "Patterns of Enterprise Application Architecture", "Domain-Driven Design", "Quantum Networking" });
            }
        }

        [Fact]
        public void TestCreateEntityUsingDefaults_AutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var mapper = AutoMapperHelpers.CreateWrapperMapper<AuthorNameDto, Author>();
                context.SetupSingleDtoAndEntities<AuthorNameDto>(false);
                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                service.Create(dto);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Single().Name.ShouldEqual("New Name");
                context.Authors.Single().Email.ShouldBeNull();
            }
        }

        [Fact]
        public void TestCreateEntityUsingDefaults_AutoMapperMappingSetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var mapper = context.SetupSingleDtoAndEntities<AuthorNameDto>(true);
                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                service.Create(dto);

                //VERIFY
                service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Single().Name.ShouldEqual("New Name");
                context.Authors.Single().Email.ShouldBeNull();
            }
        }

        //[Fact]
        //public void TestUpdateEntityOk()
        //{
        //    //SETUP
        //    var mapper = AutoMapperHelpers.CreateConfig<AuthorNameDto, Author>();
        //    var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        //    using (var context = new EfCoreContext(options))
        //    {
        //        context.Database.EnsureCreated();
        //        context.SeedDatabaseFourBooks();
        //    }
        //    using (var context = new EfCoreContext(options))
        //    {
        //        var service = new GenericService<EfCoreContext>(context, mapper);

        //        //ATTEMPT
        //        var dto = new AuthorNameDto { Name = "New Name" };
        //        service.Update(dto);

        //        //VERIFY
        //        service.HasErrors.ShouldBeFalse(string.Join("\n", service.Errors));
        //    }
        //    using (var context = new EfCoreContext(options))
        //    {
        //        context.Authors.Count().ShouldEqual(1);
        //        context.Authors.Find(1).Name.ShouldEqual("New Name");
        //    }
        //}
    }
}