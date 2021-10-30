// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestDirectAccess
    {
        [Fact]
        public void TestGetSingleOnEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = new Author { AuthorId = 1, Name = "New Name", Email = unique };
                service.CreateAndSave(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");

                context.ChangeTracker.Clear();
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

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = context.Authors.Find(1);
                author.Email = unique;
                service.UpdateAndSave(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Author");

                context.ChangeTracker.Clear();
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

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = new Author {AuthorId = 1, Name = "New Name", Email = unique};
                service.UpdateAndSave(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Author");

                context.ChangeTracker.Clear();
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestUpdateOnEntityKeyNotSetOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = new Author {Name = "New Name", Email = unique };
                var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateAndSave(author));

                //VERIFY
                ex.Message.ShouldStartWith("The primary key was not set on the entity class Author.");
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

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.DeleteAndSave<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully deleted a Book");

                context.ChangeTracker.Clear();
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

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

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

                context.ChangeTracker.Clear();
                context.Books.Count().ShouldEqual(stopDelete ? 4 : 3);
            }
        }

        [Fact]
        public void TestNotEntityNoDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex  = Assert.Throws<InvalidOperationException>(() => service.ReadSingle<string>(1));

                //VERIFY
                ex.Message.ShouldEqual("The class String is not registered as entity class in your DbContext EfCoreContext.");
            }
        }
    }
}