using System;
using System.Linq;
using AutoMapper;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Benchmarking
{
    public class PerfListMany
    {
        private WrappedAutoMapperConfig _wrapped;
        private DbContextOptions<EfCoreContext> _options;

        [Fact]
        public void ProjectToBookListDto()
        {
            var summary = BenchmarkRunner.Run<PerfListMany>();
        }

        [GlobalSetup]
        public void Setup()
        {
            var con = this.GetUniqueDatabaseConnectionString(nameof(PerfListMany));
            _options = new DbContextOptionsBuilder<EfCoreContext>().UseSqlServer(con).Options;
            using (var context = new EfCoreContext(_options))
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                    context.SeedDatabaseDummyBooks(100);
                _wrapped = context.SetupSingleDtoAndEntities<LocalBookListDto>(true);
            }
        }

        [Benchmark]
        public void RunHandCoded()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                var books = LocalBookListDto.MapBookToDto(context.Books).ToList();

                //VERIFY
                books.Count.ShouldEqual(100);
            }
        }

        [Benchmark]
        public void RunGenericService()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                var service = new GenericService<EfCoreContext>(context, _wrapped);

                //ATTEMPT
                var books = service.GetManyNoTracked<LocalBookListDto>().ToList();

                //VERIFY
                service.IsValid.ShouldBeTrue();
                books.Count.ShouldEqual(100);
            }
        }


        public class LocalBookListDto : ILinkToEntity<Book>, IConfigFoundIn<LocalBookListDtoConfig>
        {
            public int BookId { get; set; }
            public string Title { get; set; }
            public DateTime PublishedOn { get; set; }
            public decimal OrgPrice { get; set; }
            public decimal ActualPrice { get; set; }
            public string PromotionalText { get; set; }
            public string AuthorsOrdered { get; set; }

            public int ReviewsCount { get; set; }
            public double? ReviewsAverageVotes { get; set; }

            public static IQueryable<LocalBookListDto> MapBookToDto( IQueryable<Book> books)
            {
                return books.Select(p => new LocalBookListDto
                {
                    BookId = p.BookId,
                    Title = p.Title,
                    PublishedOn = p.PublishedOn,
                    ActualPrice = p.ActualPrice,
                    OrgPrice = p.OrgPrice,
                    PromotionalText = p.PromotionalText,
                    AuthorsOrdered = string.Join(", ",
                        p.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name)),
                    ReviewsCount = p.Reviews.Count(),
                    ReviewsAverageVotes = p.Reviews.Select(y =>(double?)y.NumStars).Average()
                });
            }
        }

        class LocalBookListDtoConfig : PerDtoConfig<LocalBookListDto, Book>
        {
            public override Action<IMappingExpression<Book, LocalBookListDto>> AlterReadMapping
            {
                get
                {
                    return cfg => cfg
                        .ForMember(x => x.ReviewsCount, x => x.MapFrom(book => book.Reviews.Count()))
                        .ForMember(x => x.AuthorsOrdered, y => y.MapFrom(p => string.Join(", ",
                            p.AuthorsLink.OrderBy(q => q.Order).Select(q => q.Author.Name))))
                    .ForMember(x => x.ReviewsAverageVotes,
                        x => x.MapFrom(p => p.Reviews.Select(y => (double?)y.NumStars).Average()));
                }
            }
        }

    }
}
