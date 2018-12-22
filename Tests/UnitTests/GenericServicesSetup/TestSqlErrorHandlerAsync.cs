// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Dtos;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestSqlErrorHandlerAsync
    {
        //--------------------------------------------------
        //Create

        [Fact]
        public async Task TestCreateNoSqlErrorHandler()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity {UniqueString = "Hello"});
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await  Assert.ThrowsAsync<DbUpdateException>(() =>
                    service.CreateAndSaveAsync(new UniqueEntity {UniqueString = "Hello"}));

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: UniqueEntities.UniqueString'.");
            }
        }

        [Fact]
        public async Task TestCreateCatchSqlErrorOn()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                context.SaveChanges();

                var config = new GenericServicesConfig()
                {
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.CreateAndSaveAsync(new UniqueEntity { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //Update

        [Fact]
        public async Task TestUpdateNoSqlErrorHandler()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                var entity = new UniqueEntity { UniqueString = "Goodbye" };
                context.UniqueEntities.Add(entity);
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                entity.UniqueString = "Hello";
                var ex = await Assert.ThrowsAsync<DbUpdateException>(() => service.UpdateAndSaveAsync(entity));

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: UniqueEntities.UniqueString'.");
            }
        }

        [Fact]
        public async Task TestUpdateCatchSqlErrorOn()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity { UniqueString = "Hello" });
                var entity = new UniqueEntity { UniqueString = "Goodbye" };
                context.UniqueEntities.Add(entity);
                context.SaveChanges();

                var config = new GenericServicesConfig()
                {
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                entity.UniqueString = "Hello";
                await service.UpdateAndSaveAsync(entity);

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //delete

        [Fact]
        public async Task TestDeleteNoSqlErrorHandler()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var firstBook = context.Books.First();
                var status = Order.CreateOrder("J", DateTime.Today,
                    new List<OrderBooksDto> { new OrderBooksDto(firstBook.BookId, firstBook, 1) });
                context.Add(status.Result);

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<DbUpdateException>(() => service.DeleteAndSaveAsync<Book>(firstBook.BookId));

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
            }
        }

        [Fact]
        public async Task TestDeleteCatchSqlErrorTurnedOn()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var firstBook = context.Books.First();
                var status = Order.CreateOrder("J", DateTime.Today,
                    new List<OrderBooksDto> { new OrderBooksDto(firstBook.BookId, firstBook, 1) });
                context.Add(status.Result);

                var config = new GenericServicesConfig()
                {
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<BookTitle>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.DeleteAndSaveAsync<Book>(firstBook.BookId);

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //Check the SaveChangesExceptionHandler

        [Theory]
        [InlineData(1, true)]
        [InlineData(19, false)]
        public async Task TestSqlErrorHandlerWorksOkAsync(int sqlErrorCode, bool shouldThrowException)
        {

            IStatusGeneric CatchUniqueError(Exception e, DbContext context)
            {
                var dbUpdateEx = e as DbUpdateException;
                var sqliteError = dbUpdateEx?.InnerException as SqliteException;
                return sqliteError?.SqliteErrorCode == sqlErrorCode
                    ? new StatusGenericHandler().AddError("Unique constraint failed")
                    : null;
            }

            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity {UniqueString = "Hello"});
                context.SaveChanges();

                var config = new GenericServicesConfig()
                {
                    SaveChangesExceptionHandler = CatchUniqueError
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                try
                {
                    await service.CreateAndSaveAsync(new UniqueWithConfigDto { UniqueString = "Hello"});
                }
                //VERIFY
                catch (Exception)
                {
                    shouldThrowException.ShouldBeTrue();
                    return;
                }

                shouldThrowException.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Unique constraint failed");
            }

        }

        //------------------------------------------------
        //private

        IStatusGeneric CatchUniqueError19(Exception e, DbContext context)
        {
            var dbUpdateEx = e as DbUpdateException;
            var sqliteError = dbUpdateEx?.InnerException as SqliteException;
            return sqliteError?.SqliteErrorCode == 19 
                ? new StatusGenericHandler().AddError("Unique constraint failed") 
                : null;
        }
    }
}