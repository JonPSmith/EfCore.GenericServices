using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Xunit;
using GenericServices.Internal.Decoders;
using GenericServices.Setup.Internal;
using Tests.Configs;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestCreateMapGenerator
    {
        private readonly DecodedEntityClass _bookInfo;
        public TestCreateMapGenerator()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _bookInfo = new DecodedEntityClass(typeof(Book), context);
            }
        }

        [Fact]
        public void TestAuthorReadMappings()
        {
            //SETUP
            var maps = new MapperConfigurationExpression();

            //ATTEMPT
            var mapCreator = new CreateConfigGenerator(typeof(AuthorNameDto), _bookInfo, null);
            mapCreator.Accessor.AddReadMappingToProfile(maps);

            //VERIFY
            var config = new MapperConfiguration(maps);
            var entity = new Author {AuthorId = 1, Name = "Author", Email = "me@nospam.com"};
            var dto = config.CreateMapper().Map<AuthorNameDto>(entity);
            dto.Name.ShouldEqual("Author");
        }

        [Fact]
        public void TestBookReadMappingsWithConfig()
        {
            //SETUP
            var maps = new MapperConfigurationExpression();

            //ATTEMPT
            var mapCreator = new CreateConfigGenerator(typeof(BookTitleAndCount), _bookInfo, new BookTitleAndCountConfig());
            mapCreator.Accessor.AddReadMappingToProfile(maps);

            //VERIFY
            var config = new MapperConfiguration(maps);
            var entity = DddEfTestData.CreateFourBooks().Last();
            var dto = config.CreateMapper().Map<BookTitleAndCount>(entity);
            dto.Title.ShouldEqual("Quantum Networking");
            dto.ReviewsCount.ShouldEqual(2);
        }



    }
}