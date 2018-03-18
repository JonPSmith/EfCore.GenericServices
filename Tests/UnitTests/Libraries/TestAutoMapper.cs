// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.EfClasses;
using Tests.Configs;
using Tests.Dtos;
using Tests.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Libraries
{
    public class TestAutoMapper
    {
        [Fact]
        public void TestDirectMappingBookTitle()
        {
            //SETUP
            var mapperConfig = AutoMapperHelpers.CreateConfig<Book, BookTitle>();

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().First();
            var data = mapperConfig.CreateMapper().Map<BookTitle>(input);

            //VERIFY
            data.Title.ShouldEqual("Refactoring");
        }

        [Fact]
        public void TestProjectionMappingBookTitle()
        {
            //SETUP
            var mapperConfig = AutoMapperHelpers.CreateConfig<Book, BookTitle>();

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().AsQueryable();
            var list = input.ProjectTo<BookTitle>(mapperConfig).ToList();

            //VERIFY
            list.First().Title.ShouldEqual("Refactoring");
        }

        [Fact]
        public void TestDirectMappingBookTitleAndCount()
        {
            //SETUP
            var genSerConfig = new BookTitleWithCountConfig();
            var mapperConfig = AutoMapperHelpers.CreateReadConfigWithConfig<Book, BookTitleAndCount>(genSerConfig.AlterReadMapping);

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().Last();
            var data = mapperConfig.CreateMapper().Map<BookTitleAndCount>(input);

            //VERIFY
            data.Title.ShouldEqual("Quantum Networking");
            data.ReviewsCount.ShouldEqual(2);
        }

        [Fact]
        public void TestProjectionMappingBookTitleAndCount()
        {
            //SETUP
            var mapperConfig = BookTitleAndCount.Config;

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().AsQueryable();
            var list = input.ProjectTo<BookTitleAndCount>(mapperConfig).ToList();

            //VERIFY
            list.Last().Title.ShouldEqual("Quantum Networking");
            list.Last().ReviewsCount.ShouldEqual(2);
        }

        [Fact]
        public void TestProjectionMappingBookTitleBadType()
        {
            //SETUP
            var mapperConfig = AutoMapperHelpers.CreateConfig<Book, BookTitleBadType>();

            //ATTEMPT
            mapperConfig.AssertConfigurationIsValid();

            //VERIFY
            //Doesn't error on name fit but different 
        }

        [Fact]
        public void TestDirectMappingToBookNotSetPrivateSetter()
        {
            //SETUP

            var mapperConfig = AutoMapperHelpers.CreateConfig<BookTitle, Book>();
            var entity = DddEfTestData.CreateFourBooks().First();

            //ATTEMPT
            var dto = new BookTitle {Title = "New Title"};
            var data = mapperConfig.CreateMapper().Map(dto, entity);

            //VERIFY
            data.Title.ShouldEqual("Refactoring");
        }

        [Fact]
        public void TestPacalNamingConvention()
        {
            //SETUP
            var pascal = new PascalCaseNamingConvention();

            //ATTEMPT
            var result = pascal.SplittingExpression.Replace("ThisIsPascal", "$1 ");

            //VERIFY
            result.ShouldEqual("This Is Pascal ");
        }
    }
}