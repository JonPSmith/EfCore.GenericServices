// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
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

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestDirectAccessAsync
    {
        //Dummy - not needed becasue direct, but GenericService tests to make sure its not null
        WrappedAutoMapperConfig _wrappedMapperConfig = AutoMapperHelpers.CreateWrapperMapper<Book, BookTitle>();

        [Fact]
        public async Task TestGetSingleOnEntityOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

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

                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

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

                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(99);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.Errors.First().ToString().ShouldEqual("Sorry, I could not find the Book you were looking for.");
                book.ShouldBeNull();

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

                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

                //ATTEMPT
                var books = service.ReadManyNoTracked<Book>();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                books.Count().ShouldEqual(4);
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
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

                //ATTEMPT
                var author = new Author { AuthorId = 1, Name = "New Name", Email = unique };
                await service.AddNewAndSaveAsync(author);
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
        public async Task TestUpdateOnEntityAlreadyTrackedOk()
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
                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

                //ATTEMPT
                var author = await service.ReadSingleAsync<Author>(1);
                author.Email = unique;
                await service.UpdateAndSaveAsync(author);
                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
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
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericServiceAsync(context, _wrappedMapperConfig);
                var logs = context.SetupLogging();

                //ATTEMPT
                var author = new Author {AuthorId = 1, Name = "New Name", Email = unique};
                await service.UpdateAndSaveAsync(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Find(1).Email.ShouldEqual(unique);
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
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

                //ATTEMPT
                await service.DeleteAndSaveAsync<Book>(1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(3);
            }
        }

        private async Task<IStatusGeneric> DeleteCheck(DbContext db, Book entity)
        {
            var status = new StatusGenericHandler();
            if (entity.BookId == 1)
                status.AddError("Stop delete");
            return status;
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
            }
            using (var context = new EfCoreContext(options))
            {
                var service = new GenericServiceAsync(context, _wrappedMapperConfig);

                //ATTEMPT
                await service.DeleteWithActionAndSaveAsync<Book>(DeleteCheck, bookId);

                //VERIFY
                if (bookId == 1)
                    service.IsValid.ShouldBeFalse();
                else
                    service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Books.Count().ShouldEqual(bookId == 1 ? 4 : 3);
            }
        }
    }
}