// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestReadViaDtoWithPreQuery
    {


        //------------------------------------------------------
        //Read many

        [Fact]
        public async Task TestProjectBookTitleManyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                var service = new CrudServices(context, utData.Wrapped);

                //ATTEMPT
                var list = await service.ReadManyWithPreQueryNoTracked<Book, BookTitle>(books =>
                    books.Where(x => x.AuthorsLink.Select(y => y.Author.Name).Contains("Martin Fowler"))).ToListAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                list.Count.ShouldEqual(2);
                list.Select(x => x.Title).ShouldEqual(new []{ "Refactoring", "Patterns of Enterprise Application Architecture"});
            }
        }

    }
}