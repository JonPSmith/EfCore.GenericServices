using System;
using System.Linq;
using AutoMapper;
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
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Benchmarking
{
    public class PerfCallMethodToUpdate1
    {
        private UnitTestData _utData;
        private DbContextOptions<EfCoreContext> _options;

        [Fact]
        public void AddReviewToBook()
        {
            var summary = BenchmarkRunner.Run<PerfCallMethodToUpdate1>();
        }

        [GlobalSetup]
        public void Setup()
        {
            _options = SqliteInMemory.CreateOptions<EfCoreContext>();
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
                var dto = new Tests.Dtos.AddReviewDto { BookId = 1, Comment = "comment", NumStars = 3, VoterName = "user" };
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
                var book = context.Find<Book>(1);
                book.AddReview(5, "comment", "user", context);
                context.SaveChanges();

                //VERIFY
                context.Set<Review>().Count().ShouldEqual(numReviews + 1);
            }
        }


    }
}
