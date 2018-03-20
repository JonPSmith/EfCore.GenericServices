using System.ComponentModel;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using Xunit;
using GenericServices.Internal.Decoders;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestDecodedDto
    {
        private DecodedEntityClass _bookInfo;
        public TestDecodedDto()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _bookInfo = new DecodedEntityClass(typeof(Book), context);
            }
        }

        public class Dto1 : ILinkToEntity<Book>
        {
            public int BookId { get; set; }

            [ReadOnly(true)]
            public string Title { get; set; }

            public string ImageUrl { get; set; }
        }

        [Fact]
        public void TestDecodedDto1CheckPropertyTypes()
        {
            //SETUP

            //ATTEMPT
            var decoded = new DecodedDto(typeof(Dto1), _bookInfo, new GenericServicesConfig(), null);

            //VERIFY
            decoded.LinkedToType.ShouldEqual(typeof(Book));
            decoded.PropertyInfos.Single(x => x.PropertyType.HasFlag(DtoPropertyTypes.KeyProperty)).PropertyInfo.Name.ShouldEqual(nameof(Dto1.BookId));
            decoded.PropertyInfos.Single(x => x.PropertyType == DtoPropertyTypes.Normal).PropertyInfo.Name.ShouldEqual(nameof(Dto1.ImageUrl));
            var names = decoded.PropertyInfos.Where(x => x.PropertyType.HasFlag(DtoPropertyTypes.ReadOnly)).Select(x => x.PropertyInfo.Name).ToArray();
            names.ShouldEqual(new string[]{ nameof(Dto1.BookId) , nameof(Dto1.Title) });
        }

        [Fact]
        public void TestDecodedMethodDefinedByDtoName()
        {
            //SETUP

            //ATTEMPT
            var decoded = new DecodedDto(typeof(Tests.Dtos.AddReviewDto), _bookInfo, new GenericServicesConfig(), null);

            //VERIFY
            decoded.MatchedSetterMethods.Count.ShouldEqual(1);
            decoded.MatchedSetterMethods.First().Method.Name.ShouldEqual("AddReview");
            decoded.MatchedSetterMethods.First().HowDefined.ShouldEqual(HowTheyWereAskedFor.NamedMethodFromDtoClass);
        }

        [Fact]
        public void TestDecodedMethodFoundByDefault()
        {
            //SETUP

            //ATTEMPT
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookInfo, new GenericServicesConfig(), null);

            //VERIFY
            decoded.MatchedSetterMethods.Count.ShouldEqual(2);
            decoded.MatchedSetterMethods.First().Method.Name.ShouldEqual("UpdatePublishedOn");
            decoded.MatchedSetterMethods.First().HowDefined.ShouldEqual(HowTheyWereAskedFor.DefaultMatchToProperties);
        }
    }
}