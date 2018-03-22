using System;
using System.Linq;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
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
        private WrappedAutoMapperConfig _wrapped;

        [Fact]
        public void AddReviewToBook()
        {
            var summary = BenchmarkRunner.Run<PerfCallMethodToUpdate1>();
        }

        [GlobalSetup]
        public void Setup()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _wrapped = context.SetupSingleDtoAndEntities<AddReviewDto>(true);
            }
        }

        [Benchmark]
        public void RunGenericService()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new GenericService<EfCoreContext>(context, _wrapped);

                //ATTEMPT
                var dto = new Tests.Dtos.AddReviewDto { BookId = 1, Comment = "comment", NumStars = 3, VoterName = "user" };
                service.UpdateAndSave(dto, nameof(Book.AddReview));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.Set<Review>().Count().ShouldEqual(3);
            }
        }

        [Benchmark]
        public void RunHandCoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books.First();
                book.AddReview(5, "comment", "user");
                context.SaveChanges();

                //VERIFY
                context.Set<Review>().Count().ShouldEqual(3);
            }
        }


    }
}
