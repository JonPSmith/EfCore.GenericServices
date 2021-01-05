// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestProjectFromEntityToDto
    {
        private readonly ITestOutputHelper _output;

        public TestProjectFromEntityToDto(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestProjectFromEntityToDtoOk()
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
                var dto = await service.ProjectFromEntityToDto<Book,BookTitle>(x => x.Where(y => y.BookId == 1)).SingleAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.BookId.ShouldEqual(1);
                dto.Title.ShouldEqual("Refactoring");
            }
        }

        [Fact]
        public async Task TestProjectFromEntityToDtoIgnoreQueryFiltersOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var author = new SoftDelEntity { SoftDeleted = true };
                context.Add(author);
                context.SaveChanges();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<SoftDelEntityDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                context.SoftDelEntities.Count().ShouldEqual(0);
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(1);

                //ATTEMPT
                var dto = await service.ProjectFromEntityToDto<SoftDelEntity, SoftDelEntityDto>(x => x.IgnoreQueryFilters().Where(y => y.Id == 1))
                    .SingleAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.Id.ShouldEqual(1);
            }

        }
    }
}