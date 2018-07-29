// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using Tests.Helpers;

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestReadSingleNull
    {
        private IGenericServicesConfig _configNoError = new GenericServicesConfig
        {
            NoErrorOnReadSingleNull = true
        };

        [Theory]
        [InlineData(1, true)]
        [InlineData(99, false)]
        public void TestReadSingleNullIsError(int bookId, bool shouldBeValid)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = service.ReadSingle<Book>(bookId);

                //VERIFY
                service.IsValid.ShouldEqual(shouldBeValid);
                if (!service.IsValid)
                    service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book you were looking for.");
            }
        }

        [Theory]
        [InlineData(1, "Success")]
        [InlineData(99, "The Book was not found.")]
        public void TestReadSingleNullIsNotError(int bookId, string message)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>(_configNoError);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = service.ReadSingle<Book>(bookId);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual(message);
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(99, false)]
        public void TestReadSingleWhereNullIsError(int bookId, bool shouldBeValid)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = service.ReadSingle<Book>(x => x.BookId == bookId);

                //VERIFY
                service.IsValid.ShouldEqual(shouldBeValid);
                if (!service.IsValid)
                    service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book you were looking for.");
            }
        }

        [Theory]
        [InlineData(1, "Success")]
        [InlineData(99, "The Book was not found.")]
        public void TestReadSingleWhereNullIsNotError(int bookId, string message)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>(_configNoError);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = service.ReadSingle<Book>(x => x.BookId == bookId);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual(message);
            }
        }

        //------------------------------------------------------------------
        //Async


        [Theory]
        [InlineData(1, true)]
        [InlineData(99, false)]
        public async Task TestReadSingleNullIsErrorAsync(int bookId, bool shouldBeValid)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(bookId);

                //VERIFY
                service.IsValid.ShouldEqual(shouldBeValid);
                if (!service.IsValid)
                    service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book you were looking for.");
            }
        }

        [Theory]
        [InlineData(1, "Success")]
        [InlineData(99, "The Book was not found.")]
        public async Task TestReadSingleNullIsNotErrorAsync(int bookId, string message)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>(_configNoError);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(bookId);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual(message);
            }
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(99, false)]
        public async Task TestReadSingleWhereNullIsErrorAsync(int bookId, bool shouldBeValid)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(x => x.BookId == bookId);

                //VERIFY
                service.IsValid.ShouldEqual(shouldBeValid);
                if (!service.IsValid)
                    service.GetAllErrors().ShouldEqual("Sorry, I could not find the Book you were looking for.");
            }
        }

        [Theory]
        [InlineData(1, "Success")]
        [InlineData(99, "The Book was not found.")]
        public async Task TestReadSingleWhereNullIsNotErrorAsync(int bookId, string message)
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>(_configNoError);
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var book = await service.ReadSingleAsync<Book>(x => x.BookId == bookId);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual(message);
            }
        }
    }
}