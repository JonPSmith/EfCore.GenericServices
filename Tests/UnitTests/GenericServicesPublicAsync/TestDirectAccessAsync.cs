// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestDirectAccessAsync
    {
        [Fact]
        public async Task TestGetSingleOnEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                book.BookId.ShouldEqual(1);
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public async Task TestGetSingleOnEntityWhereOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(x => x.BookId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                book.BookId.ShouldEqual(1);
                context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            }
        }

        [Fact]
        public async Task TestGetSingleOnEntityNotFound()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(99);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.First().ToString().ShouldEqual("Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();
            }
        }

        [Fact]
        public async Task TestGetSingleOnEntityWhereBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(x => x.BookId == 99);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.First().ToString().ShouldEqual("Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();
            }
        }

        [Fact]
        public async Task TestGetSingleOnEntityWhereException()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReadSingleAsync<Book>(x => true));

                //VERIFY
                ex.Message.ShouldEqual("Sequence contains more than one element.");
            }
        }

        [Fact]
        public async Task TestManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var books = await service.ReadManyNoTracked<Book>().ToListAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                books.Count.ShouldEqual(4);
                context.Entry(books.ToList().First()).State.ShouldEqual(EntityState.Detached);
            }
        }

        [Fact]
        public async Task TestCreateEntityOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = new Author { AuthorId = 1, Name = "New Name", Email = unique };
                await service.CreateAndSaveAsync(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

                context.ChangeTracker.Clear();
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public async Task TestUpdateOnEntityAlreadyTrackedOk()
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
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = await service.ReadSingleAsync<Author>(1);
                author.Email = unique;
                await service.UpdateAndSaveAsync(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

                context.ChangeTracker.Clear();
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public async Task TestUpdateOnEntityNotTrackedOk()
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
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var author = new Author {AuthorId = 1, Name = "New Name", Email = unique};
                await service.UpdateAndSaveAsync(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

                context.ChangeTracker.Clear();
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public async Task TestUpdateOnEntityKeyNotSetOk()
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
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);;

                //ATTEMPT
                var author = new Author { Name = "New Name", Email = unique };
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAndSaveAsync(author));

                //VERIFY
                ex.Message.ShouldStartWith("The primary key was not set on the entity class Author.");
            }
        }

        [Fact]
        public async Task TestDeleteEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.DeleteAndSaveAsync<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());

                context.ChangeTracker.Clear();
                context.Books.Count().ShouldEqual(3);
            }
        }

        private async Task<IStatusGeneric> DeleteCheck(DbContext db, Book entity)
        {
            var status = new StatusGenericHandler();
            if (entity.BookId == 1)
                status.AddError("Stop delete");
            return await Task.FromResult(status);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async Task TestDeleteWithActionEntityOk(int bookId)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.DeleteWithActionAndSaveAsync<Book>(DeleteCheck, bookId);

                //VERIFY
                if (bookId == 1)
                    service.IsValid.ShouldBeFalse();
                else
                    service.IsValid.ShouldBeTrue(service.GetAllErrors());

                context.ChangeTracker.Clear();
                context.Books.Count().ShouldEqual(bookId == 1 ? 4 : 3);
            }
        }

        [Fact]
        public async Task TestNotEntityNoDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReadSingleAsync<string>(1));

                //VERIFY
                ex.Message.ShouldEqual("The class String is not registered as entity class in your DbContext EfCoreContext.");
            }
        }
    }
}