// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
using ServiceLayer.HomeController.Dtos;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Performance
{
    public class TestReadSingleAgainstReadSsingleToExisting
    {
        private readonly ITestOutputHelper _output;

        public TestReadSingleAgainstReadSsingleToExisting(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestProjectBookListDtoSingleOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookListDto>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var preload = service.ReadSingle<BookListDto>(1);
                using (new TimeThings(_output, "ReadSingle", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var dto = service.ReadSingle<BookListDto>(1);
                    }
                }
                using (new TimeThings(_output, "ReadSingleToDto", 100))
                {
                    var dto = new BookListDto();
                    for (int i = 0; i < 100; i++)
                    {
                        service.ReadSingleToDto(dto, 1);
                    }
                }
                using (new TimeThings(_output, "ReadSingle", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var dto = service.ReadSingle<BookListDto>(1);
                    }
                }
                using (new TimeThings(_output, "ReadSingleToDto", 100))
                {
                    var dto = new BookListDto();
                    for (int i = 0; i < 100; i++)
                    {
                        service.ReadSingleToDto(dto, 1);
                    }
                }
            }
        }

        [Fact]
        public void TestProjectBookTitleSingleOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var mapper = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new GenericService(context, mapper);

                //ATTEMPT
                var preload = service.ReadSingle<BookTitle>(1);
                using (new TimeThings(_output, "ReadSingle", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var dto = service.ReadSingle<BookTitle>(1);
                    }
                }
                using (new TimeThings(_output, "ReadSingleToDto", 100))
                {
                    var dto = new BookTitle();
                    for (int i = 0; i < 100; i++)
                    {
                        service.ReadSingleToDto(dto, 1);
                    }
                }
                using (new TimeThings(_output, "ReadSingle", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var dto = service.ReadSingle<BookTitle>(1);
                    }
                }
                using (new TimeThings(_output, "ReadSingleToDto", 100))
                {
                    var dto = new BookTitle();
                    for (int i = 0; i < 100; i++)
                    {
                        service.ReadSingleToDto(dto, 1);
                    }
                }
            }
        }


    }
}