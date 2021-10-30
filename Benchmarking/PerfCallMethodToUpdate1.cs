// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
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

namespace Benchmarking
{
    public class PerfCallMethodToUpdate1
    {
        private DbContextOptions<EfCoreContext> _options;
        private SpecificUseData _utData;

        [Fact]
        public void AddReviewToBook()
        {
            var summary = BenchmarkRunner.Run<PerfCallMethodToUpdate1>();
        }

        [GlobalSetup]
        public void Setup()
        {
            _options = SqliteHelper.GetSqliteInMemoryOptions();
            using (var context = new EfCoreContext(_options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                _utData = context.SetupSingleDtoAndEntities<AddReviewDto>();
            }
        }

        [Benchmark]
        public void RunGenericService()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var service = new CrudServices<EfCoreContext>(context, _utData.ConfigAndMapper);
                var numReviews = context.Set<Review>().Count();

                //ATTEMPT
                var dto = new AddReviewDto { BookId = 1, Comment = "comment", NumStars = 3, VoterName = "user" };
                service.UpdateAndSave(dto, nameof(Book.AddReview));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.Set<Review>().Count().ShouldEqual(numReviews+1);
            }
        }

        [Benchmark]
        public void RunHandCoded()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var numReviews = context.Set<Review>().Count();

                //ATTEMPT
                var book = context.Books.Single(x => x.BookId == 1);
                book.AddReview(5, "comment", "user", context);
                context.SaveChanges();

                //VERIFY
                context.Set<Review>().Count().ShouldEqual(numReviews + 1);
            }
        }
    }
}
