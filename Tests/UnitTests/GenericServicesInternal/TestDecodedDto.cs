using System;
using System.ComponentModel;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration.Internal;
using Xunit;
using GenericServices.Internal.Decoders;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestDecodedDto
    {
        private DecodedEntityClass _bookEntityInfo;
        public TestDecodedDto()
        {
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                _bookEntityInfo = new DecodedEntityClass(typeof(Book), context);
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
            var decoded = new DecodedDto(typeof(Dto1), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

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
            var decoded = new DecodedDto(typeof(Tests.Dtos.AddReviewDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

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
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

            //VERIFY
            decoded.MatchedSetterMethods.Count.ShouldEqual(2);
            decoded.MatchedSetterMethods.First().Method.Name.ShouldEqual("UpdatePublishedOn");
            decoded.MatchedSetterMethods.First().HowDefined.ShouldEqual(HowTheyWereAskedFor.DefaultMatchToProperties);
        }

        //-----------------------------------------------------------
        //DecodedDto methods

        [Fact]
        public void TestFindSetterMethod()
        {
            //SETUP
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

            //ATTEMPT
            var method = decoded.FindSetterMethod(new DecodeName("UpdatePublishedOn"));

            //VERIFY
            method.Method.Name.ShouldEqual("UpdatePublishedOn");
        }

        [Fact]
        public void TestFindSetterMethodWithGivenNumParams()
        {
            //SETUP
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

            //ATTEMPT
            var method = decoded.FindSetterMethod(new DecodeName("UpdatePublishedOn(1)"));

            //VERIFY
            method.Method.Name.ShouldEqual("UpdatePublishedOn");
        }

        [Fact]
        public void TestFindSetterMethodBadName()
        {
            //SETUP
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => decoded.FindSetterMethod(new DecodeName("badname")));

            //VERIFY
            ex.Message.ShouldStartWith("Could not find a method of name badname");
        }

        [Fact]
        public void TestFindSetterMethodNumParamsBad()
        {
            //SETUP
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => decoded.FindSetterMethod(new DecodeName("UpdatePublishedOn(2)")));

            //VERIFY
            ex.Message.ShouldStartWith("Could not find a method of name UpdatePublishedOn(2)");
        }

        [Fact]
        public void TestGetDefaultSetterMethod()
        {
            //SETUP
            var decoded = new DecodedDto(typeof(Tests.Dtos.ChangePubDateDto), _bookEntityInfo, new ExpandedGlobalConfig(null, null), null);

            //ATTEMPT
            var method = decoded.GetDefaultSetterMethod(_bookEntityInfo);

            //VERIFY
            method.Method.Name.ShouldEqual("UpdatePublishedOn");
            decoded.MatchedSetterMethods.Count.ShouldEqual(2); //It ignores the parameterless RemovePromotion method
        }

    }
}