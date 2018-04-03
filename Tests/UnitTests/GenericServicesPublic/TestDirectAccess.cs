// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
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

                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = service.ReadSingle<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                book.BookId.ShouldEqual(1);
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public void TestGetSingleOnEntityWhereOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = service.ReadSingle<Book>(x => x.BookId == 1);

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

                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = service.ReadSingle<Book>(99);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.First().ToString().ShouldEqual("Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();
            }
        }

        [Fact]
        public void TestGetSingleOnEntityWhereBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = service.ReadSingle<Book>(x => x.BookId == 99);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.First().ToString().ShouldEqual("Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();
            }
        }

        [Fact]
        public void TestGetSingleOnEntityWhereException()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => service.ReadSingle<Book>(x => true));

                //VERIFY
                ex.Message.ShouldEqual("Sequence contains more than one element");
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

                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var books = service.ReadManyNoTracked<Book>();

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
                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var author = new Author { AuthorId = 1, Name = "New Name", Email = unique };
                service.AddNewAndSave(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");
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
                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                var author = context.Authors.Find(1);
                author.Email = unique;
                service.UpdateAndSave(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated a Author");
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
                var service = new GenericService(context, _wrappedMapperConfig);
                var logs = context.SetupLogging();

                //ATTEMPT
                var author = new Author {AuthorId = 1, Name = "New Name", Email = unique};
                service.UpdateAndSave(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated a Author");
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
                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                service.DeleteAndSave<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully deleted a Book");
            }
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(3);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestDeleteWithActionEntityOk(bool stopDelete)
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
                var service = new GenericService(context, _wrappedMapperConfig);

                //ATTEMPT
                service.DeleteWithActionAndSave<Book>( (c,e) =>
                {
                    var status = new StatusGenericHandler();
                    if (stopDelete)
                        status.AddError("Stop delete");
                    return status;
                }, 1);

                //VERIFY
                if (stopDelete)
                    service.IsValid.ShouldBeFalse();
                else
                {
                    service.IsValid.ShouldBeTrue(service.GetAllErrors());
                    service.Message.ShouldEqual("Successfully deleted a Book");
                }
            }
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(stopDelete ? 4 : 3);
            }
        }
    }
}