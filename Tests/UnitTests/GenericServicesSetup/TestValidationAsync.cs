// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Dtos;
using DataLayer.EfClasses;
using DataLayer.EfCode;
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
    public class TestValidationAsync
    {
        //--------------------------------------------------
        //Create

        [Fact]
        public async Task TestCreateValidationNotTurnedOn()
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
        public async Task TestDirectAccessCreateWithValidationTurnedOn()
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
                    DirectAccessValidateOnSave = true,
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.CreateAndSaveAsync(new UniqueEntity { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public async Task TestDtoAccessCreateWithValidationTurnedOnViaPerDtoConfig()
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
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.CreateAndSaveAsync(new UniqueWithConfigDto() { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public async Task TestDtoAccessCreateWithValidationTurnedOnViaGlobalConfig()
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
                    DtoAccessValidateOnSave = true,
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueNoConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.CreateAndSaveAsync(new UniqueNoConfigDto() { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //Update

        [Fact]
        public async Task TestUpdateValidationNotTurnedOn()
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
        public async Task TestDirectAccessUpdateWithValidationTurnedOn()
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
                    DirectAccessValidateOnSave = true,
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                entity.UniqueString = "Hello";
                await service.UpdateAndSaveAsync(entity);

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public async Task TestDtoAccessUpdateWithValidationTurnedOnViaPerDtoConfig()
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
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.UpdateAndSaveAsync(new UniqueWithConfigDto() {Id = entity.Id, UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public async Task TestDtoAccessUpdateWithValidationTurnedOnViaGlobalConfig()
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
                    DtoAccessValidateOnSave = true,
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueNoConfigDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.UpdateAndSaveAsync(new UniqueNoConfigDto() { Id = entity.Id, UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //delete

        [Fact]
        public async Task TestDeleteWithValidationNotTurnedOn()
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
        public async Task TestDeleteWithValidationTurnedOnViaGlobalConfig()
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
                    DirectAccessValidateOnSave = true,
                    SqlErrorHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<BookTitle>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                await service.DeleteAndSaveAsync<Book>(firstBook.BookId);

                //VERIFY
                service.GetAllErrors().ShouldEqual("Line Item: Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //Check the SqlErrorHandler

        [Theory]
        [InlineData(1, true)]
        [InlineData(19, false)]
        public async Task TestSqlErrorHandlerWorksOkAsync(int sqlErrorCode, bool shouldThrowException)
        {
            ValidationResult CatchUniqueError(DbUpdateException e)
            {
                var sqliteError = e.InnerException as SqliteException;
                if (sqliteError?.SqliteErrorCode == sqlErrorCode)
                    return new ValidationResult("Unique constraint failed");
                return null;
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
                    SqlErrorHandler = CatchUniqueError
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
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }

        }

        //------------------------------------------------
        //private

        ValidationResult CatchUniqueError19(DbUpdateException e)
        {
            var sqliteError = e.InnerException as SqliteException;
            if (sqliteError?.SqliteErrorCode == 19)
                return new ValidationResult("Unique constraint failed");
            return null;
        }
    }
}