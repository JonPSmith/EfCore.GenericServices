// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestSqlErrorHander
    {
        [Fact]
        public void TestSqlErrorHandlerNotInPlace()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.UniqueEntities.Add(new UniqueEntity {UniqueString = "Hello"});
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<UniqueDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = Assert.Throws<DbUpdateException>(() =>
                    service.CreateAndSave(new UniqueEntity {UniqueString = "Hello"}));

                //VERIFY
                ex.InnerException.Message.ShouldEqual(
                    "SQLite Error 19: 'UNIQUE constraint failed: UniqueEntities.UniqueString'.");
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(19, false)]
        public void TestSqlErrorHandlerWorksOk(int sqlErrorCode, bool shouldThrowException)
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
                var utData = context.SetupSingleDtoAndEntities<UniqueDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                try
                {
                    service.CreateAndSave(new UniqueDto {UniqueString = "Hello"});
                }
                //VERIFY
                catch (Exception e)
                {
                    shouldThrowException.ShouldBeTrue();
                    return;
                }

                shouldThrowException.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }
        }

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
                var utData = context.SetupSingleDtoAndEntities<UniqueDto>(config);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                try
                {
                    await service.CreateAndSaveAsync(new UniqueDto { UniqueString = "Hello"});
                }
                //VERIFY
                catch (Exception e)
                {
                    shouldThrowException.ShouldBeTrue();
                    return;
                }

                shouldThrowException.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Unique Entity: Unique constraint failed");
            }

        }
    }
}