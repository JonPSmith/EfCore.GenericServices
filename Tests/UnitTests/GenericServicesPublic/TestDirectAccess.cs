// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestDirectAccess
    {
        //Dummy - not needed becasue direct, but GenericService tests to make sure its not null
        WrappedAutoMapperConfig _wrappedMapperConfig = AutoMapperHelpers.CreateWrapperMapper<Book, BookTitle>();

        [Fact]
        public void TestGetSingleOnEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = service.GetSingle<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                book.BookId.ShouldEqual(1);
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public void TestGetSingleOnEntityNotFound()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = service.GetSingle<Book>(99);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.First().ToString().ShouldEqual("GetSingle>Find: Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();

            }
        }

        [Fact]
        public void TestManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);

                //ATTEMPT
                var books = service.GetManyNoTracked<Book>();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                books.Count().ShouldEqual(4);
                context.Entry(books.ToList().First()).State.ShouldEqual(EntityState.Detached);
            }
        }

        [Fact]
        public void TestCreateEntityOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);

                //ATTEMPT
                var author = new Author { AuthorId = 1, Name = "New Name", Email = unique };
                service.AddNewAndSave(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestUpdateOnEntityAlreadyTrackedOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);

                //ATTEMPT
                var author = service.GetSingle<Author>(1);
                author.Email = unique;
                service.UpdateAndsave(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestUpdateOnEntityNotTrackedOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);
                var logs = context.SetupLogging();

                //ATTEMPT
                var author = new Author {AuthorId = 1, Name = "New Name", Email = unique};
                service.UpdateAndsave(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestDeleteEntityOk()
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
                var service = new GenericService<EfCoreContext>(context, _wrappedMapperConfig);

                //ATTEMPT
                service.DeleteAndSave<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(3);
            }
        }
    }
}