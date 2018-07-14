// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using Tests.Helpers;

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestValidationSync
    {
        //--------------------------------------------------
        //Create

        [Fact]
        public void TestCreateValidationNotTurnedOn()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity {UniqueString = "Hello"});
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = Assert.Throws<DbUpdateException>(() =>
                    service.CreateAndSave(new UniqueEntity {UniqueString = "Hello"}));

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: UniqueEntities.UniqueString'.");
            }
        }

        [Fact]
        public void TestDirectAccessCreateWithValidationTurnedOn()
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
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.CreateAndSave(new UniqueEntity { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public void TestDtoAccessCreateWithValidationTurnedOnViaPerDtoConfig()
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
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.CreateAndSave(new UniqueWithConfigDto() { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public void TestDtoAccessCreateWithValidationTurnedOnViaGlobalConfig()
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
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueNoConfigDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.CreateAndSave(new UniqueNoConfigDto() { UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public void TestDirectAccessInvalidOperationExceptionOnSameKey()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.KeyIsStrings.Add(new KeyIsString {Id = "Hello"});
                context.SaveChanges();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<UniqueNoConfigDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = Assert.Throws<DbUpdateException>(() =>
                    service.CreateAndSave(new KeyIsString { Id = "Hello" }));

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: KeyIsStrings.Id'.");
            }
        }

        //-------------------------------------------------------------------
        //Update

        [Fact]
        public void TestUpdateValidationNotTurnedOn()
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
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                entity.UniqueString = "Hello";
                var ex = Assert.Throws<DbUpdateException>(() => service.UpdateAndSave(entity));

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: UniqueEntities.UniqueString'.");
            }
        }

        [Fact]
        public void TestDirectAccessUpdateWithValidationTurnedOn()
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
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                entity.UniqueString = "Hello";
                service.UpdateAndSave(entity);

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public void TestDtoAccessUpdateWithValidationTurnedOnViaPerDtoConfig()
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
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.UpdateAndSave(new UniqueWithConfigDto() {Id = entity.Id, UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        [Fact]
        public void TestDtoAccessUpdateWithValidationTurnedOnViaGlobalConfig()
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
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueNoConfigDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.UpdateAndSave(new UniqueNoConfigDto() { Id = entity.Id, UniqueString = "Hello" });

                //VERIFY
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //delete

        [Fact]
        public void TestDeleteWithValidationNotTurnedOn()
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
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = Assert.Throws<DbUpdateException>(() => service.DeleteAndSave<Book>(firstBook.BookId));

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
            }
        }

        [Fact]
        public void TestDeleteWithValidationTurnedOnViaGlobalConfig()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                var firstBook = context.Books.First();
                var status = Order.CreateOrder("J", DateTime.Today,
                    new List<OrderBooksDto> {new OrderBooksDto(firstBook.BookId, firstBook, 1)});
                context.Add(status.Result);

                var config = new GenericServicesConfig()
                {
                    DirectAccessValidateOnSave = true,
                    SaveChangesExceptionHandler = CatchUniqueError19
                };
                var utData = context.SetupSingleDtoAndEntities<BookTitle>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.DeleteAndSave<Book>(firstBook.BookId);

                //VERIFY
                service.GetAllErrors().ShouldEqual("Line Item: Unique constraint failed");
            }
        }

        //-------------------------------------------------------------------
        //Check the SaveChangesExceptionHandler

        [Theory]
        [InlineData(1, true)]
        [InlineData(19, false)]
        public void TestSqlErrorHandlerWorksOk(int sqlErrorCode, bool shouldThrowException)
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
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                try
                {
                    service.CreateAndSave(new UniqueWithConfigDto {UniqueString = "Hello"});
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

        private IStatusGeneric CatchUniqueError19(Exception e, DbContext context)
        {
            var dbUpdateEx = e as DbUpdateException;
            var sqliteError = dbUpdateEx?.InnerException as SqliteException;
            return sqliteError?.SqliteErrorCode == 19
                ? new StatusGenericHandler().AddError("Unique constraint failed")
                : null;
        }
    }
}