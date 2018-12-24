// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Performance
{
    public class CompareAgainstHandCoded
    {
        private readonly ITestOutputHelper _output;

        public CompareAgainstHandCoded(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPerformanceReadSingleDirect()
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
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices<EfCoreContext>(context, utData.ConfigAndMapper);

                using (new TimeThings(_output, "RunHandCoded Find", 1))
                {
                    context.Find<Book>(1);
                }

                using (new TimeThings(_output, "RunGenericService Find", 1))
                {
                    service.ReadSingle<Book>(1);
                }

                using (new TimeThings(_output, "RunGenericService Find", 1000))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        service.ReadSingle<Book>(1);
                    }
                }

                using (new TimeThings(_output, "RunHandCoded Find", 1000))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        context.Find<Book>(1);
                    }
                }
            }
        }

        [Fact]
        public void TestPerformanceCreateDirectViaCtor()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices<TestDbContext>(context, utData.ConfigAndMapper);

                using (new TimeThings(_output, "RunHandCoded Create", 1))
                {
                    context.Add(new DddCtorEntity( 1, "hello"));
                    context.SaveChanges();
                }

                using (new TimeThings(_output, "RunGenericService Create", 1))
                {
                    service.CreateAndSave(new DddCtorEntity( 1, "hello"));
                }
            }

            using (var context = new TestDbContext(options))
            {
                context.WipeAllDataFromDatabase();
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices<TestDbContext>(context, utData.ConfigAndMapper);

                using (new TimeThings(_output, "RunGenericService Create", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        service.CreateAndSave(new DddCtorEntity(1, "hello"));
                    }
                }
            }

            using (var context = new TestDbContext(options))
            {
                context.WipeAllDataFromDatabase();
                using (new TimeThings(_output, "RunHandCoded Create", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        context.Add(new DddCtorEntity(1, "hello"));
                        context.SaveChanges();
                    }
                }
            }

            using (var context = new TestDbContext(options))
            {
                context.WipeAllDataFromDatabase();
                var utData = context.SetupEntitiesDirect();
                using (new TimeThings(_output, "new CrudServices<TestDbContext>", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var service = new CrudServices<TestDbContext>(context, utData.ConfigAndMapper);
                    }
                }
            }
        }

        [Fact]
        public void TestPerformanceAddReview()
        {
            var options1 = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options1))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options1))
            {
                var utData = context.SetupSingleDtoAndEntities<AddReviewDto>();
                var service = new CrudServices<EfCoreContext>(context, utData.ConfigAndMapper);
                using (new TimeThings(_output, "RunHandCoded AddReview", 1))
                {
                    RunHandCodedAddReview(context);
                }

                using (new TimeThings(_output, "RunGenericService AddReview", 1))
                {
                    RunGenericServiceAddReview(service);
                }

                using (new TimeThings(_output, "RunGenericService AddReview", 100))
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM reviews");
                    for (int i = 0; i < 100; i++)
                    {
                        RunGenericServiceAddReview(service);
                    }
                }
            }
            var options2 = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options2))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options2))
            {
                using (new TimeThings(_output, "RunHandCoded AddReview", 100))
                {
                    for (int i = 0; i < 100; i++)
                    {
                        RunHandCodedAddReview(context);
                    }
                }
            }
        }

        private void RunGenericServiceAddReview(CrudServices<EfCoreContext> service)
        {
            //SETUP

            //ATTEMPT
            var dto = new AddReviewDto {BookId = 1, Comment = "comment", NumStars = 3, VoterName = "user"};
            service.UpdateAndSave(dto, nameof(Book.AddReview));

            //VERIFY
            service.IsValid.ShouldBeTrue(service.GetAllErrors());
        }

        private void RunHandCodedAddReview(EfCoreContext context)
        {
            //SETUP

            //ATTEMPT
            var book = context.Find<Book>(1);
            book.AddReview(5, "comment", "user", context);
            context.SaveChanges();

            //VERIFY
        }
    }
}