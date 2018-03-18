using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMapper;
using AutoMapper.Configuration;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using Xunit;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using Tests.Dtos;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestCreateMapGenerator
    {
        private DecodedEntityClass _bookInfo;
        private DecodedEntityClass _AuthorInfo;
        public TestCreateMapGenerator()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _bookInfo = new DecodedEntityClass(typeof(Book), context);
                _AuthorInfo = new DecodedEntityClass(typeof(Author), context);
            }
        }

        [Fact]
        public void TestAuthorBuildMappings()
        {
            //SETUP
            var decodedDto = new DecodedDto(typeof(AuthorNameDto), _AuthorInfo);
            var maps = new MapperConfigurationExpression();

            //ATTEMPT
            var mapCreator = new CreateMapGenerator(decodedDto, _bookInfo, null, null);
            mapCreator.Accessor.BuildReadMapping(maps);

            //VERIFY
            var config = new MapperConfiguration(maps);
            var entity = new Author {AuthorId = 1, Name = "Author", Email = "me@nospam.com"};
            //var dto = new AuthorNameDto { Name = "New Name" };
            var dto = config.CreateMapper().Map<AuthorNameDto>(entity);
            dto.Name.ShouldEqual("Author");
        }

        

    }
}