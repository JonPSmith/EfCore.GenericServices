// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Tests.UnitTests.GenericServicesPublic;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using StackExchange.Profiling;
using Tests.Dtos;
using Tests.EfCode;
using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests.UnitTests.Performance
{
    public class ProfileGenericServices
    {
        private readonly ITestOutputHelper _output;

        public ProfileGenericServices(ITestOutputHelper output)
        {
            _output = output;
        }

        private static readonly ConcurrentDictionary<Type, string> ConDict = new ConcurrentDictionary<Type, string>();


        [Fact]
        public void TestProfileConcurrentDictionaryOk()
        {
            //SETUP
            ConDict[typeof(int)] = "Hello";
            var preResult = ConDict[typeof(int)];

            //ATTEMPT
            using (new TimeThings(_output, "First Lookup (direct)"))
            {
                var result = ConDict[typeof(int)];
            }
            using (new TimeThings(_output, "First TryGetValue"))
            {
                ConDict.TryGetValue(typeof(int), out var result);
            }

            using (new TimeThings(_output, "Next 10 TryGetValue", 10))
            {
                for (int i = 0; i < 10; i++)
                {
                    ConDict.TryGetValue(typeof(int), out var result);
                }
            }


        }

        [Fact]
        public void TestCreateEntityViaDtoCtorCreateCtor1Ok()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<TestCreateViaDto.DtoCtorCreate>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                
                //using (CrudServices.Profiler.Step("Run 1"))
                {
                    var dto = new TestCreateViaDto.DtoCtorCreate { MyInt = 123, MyString = "Test" };
                    service.CreateAndSave(dto, "ctor(1)");
                }
                CrudServices.Profiler = MiniProfiler.StartNew(nameof(TestCreateEntityViaDtoCtorCreateCtor1Ok));
                using (CrudServices.Profiler.Step("Run 2"))
                {
                    var dto = new TestCreateViaDto.DtoCtorCreate { MyInt = 123, MyString = "Test" };
                    service.CreateAndSave(dto, "ctor(1)");
                }
                _output.WriteLine(CrudServices.Profiler.RenderPlainText());

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Ddd Ctor Entity");
            }
        }

        [Fact]
        public void TestUpdateViaDefaultMethodOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                CrudServices.Profiler = MiniProfiler.StartNew(nameof(TestUpdateViaDefaultMethodOk));
                using (CrudServices.Profiler.Step("Run 1"))
                {
                    var dto = new ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                    service.UpdateAndSave(dto);
                }
                using (CrudServices.Profiler.Step("Run 2"))
                {
                    var dto = new ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                    service.UpdateAndSave(dto);
                }
                _output.WriteLine(CrudServices.Profiler.RenderPlainText());

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
            }
        }

        [Fact]
        public void TestUpdatePublicationDateViaAutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                CrudServices.Profiler = MiniProfiler.StartNew(nameof(TestUpdatePublicationDateViaAutoMapperOk));
                //using (CrudServices.Profiler.Step("Run 1"))
                {
                    var dto = new Tests.Dtos.ChangePubDateDto {BookId = 4, PublishedOn = new DateTime(2000, 1, 1)};
                    service.UpdateAndSave(dto, CrudValues.UseAutoMapper);
                }
                using (CrudServices.Profiler.Step("Run 2"))
                {
                    var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                    service.UpdateAndSave(dto, CrudValues.UseAutoMapper);
                }
                _output.WriteLine(CrudServices.Profiler.RenderPlainText());

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
            }
        }
    }
}