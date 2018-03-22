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
    public class PerfCallMethodUpdate2
    {
        private WrappedAutoMapperConfig _wrapped;

        [Fact]
        public void ChangePublicationDate()
        {
            var summary = BenchmarkRunner.Run<PerfCallMethodUpdate2>();
        }

        [GlobalSetup]
        public void Setup()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _wrapped = context.SetupSingleDtoAndEntities<Tests.Dtos.ChangePubDateDto>(true);
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
                var dto = new Tests.Dtos.ChangePubDateDto { BookId = 4, PublishedOn = new DateTime(2000, 1, 1) };
                service.UpdateAndSave(dto, nameof(Book.UpdatePublishedOn));

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                var entity = context.Books.Find(4);
                entity.PublishedOn.ShouldEqual(new DateTime(2000, 1, 1));
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
                var book = context.Books.Find(4);
                book.UpdatePublishedOn(new DateTime(2000, 1, 1));
                context.SaveChanges();

                //VERIFY
                var entity = context.Books.Find(4);
                entity.PublishedOn.ShouldEqual(new DateTime(2000, 1, 1));
            }
        }


    }
}
