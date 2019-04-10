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
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController.Dtos;
using ServiceLayer.HomeController.QueryObjects;
using Tests.Helpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Benchmarking
{
    public class PerfListMany
    {
        private IWrappedConfigAndMapper _config;
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
                var utData = context.SetupSingleDtoAndEntities<BookListDto>();
                _config = utData.ConfigAndMapper;
            }
        }

        [Benchmark]
        public void RunHandCoded()
        {
            //SETUP
            using (var context = new EfCoreContext(_options))
            {
                //ATTEMPT
                var books = context.Books.AsNoTracking().MapBookToDto().ToList();

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
                var service = new CrudServices<EfCoreContext>(context, _config, new CreateNewDBContextHelper(() => new EfCoreContext(_options)));

                //ATTEMPT
                var books = service.ReadManyNoTracked<BookListDto>().ToList();

                //VERIFY
                books.Count.ShouldEqual(100);
            }
        }


    }
}
