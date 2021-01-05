// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestDecodedDtoKeyIsString
    {
        private DecodedEntityClass _EntityInfo;

        public TestDecodedDtoKeyIsString()
        {
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                _EntityInfo = new DecodedEntityClass(typeof(DddCompositeIntString), context);
            }
        }

        [Fact]
        public void TestDecodedKeyIsStringCheckPropertyTypes()
        {
            //SETUP

            //ATTEMPT
            var decoded = new DecodedDto(typeof(DddCompositeIntStringCreateDto), _EntityInfo, new GenericServicesConfig(), null);

            //VERIFY
            decoded.LinkedEntityInfo.EntityType.ShouldEqual(typeof(DddCompositeIntString));
            decoded.PropertyInfos.Where(x => x.PropertyType.HasFlag(DtoPropertyTypes.KeyProperty))
                .Select( x => x.PropertyInfo.Name).ToArray()
                .ShouldEqual(new[] { nameof(DddCompositeIntStringCreateDto.MyString), nameof(DddCompositeIntStringCreateDto.MyInt) });
            var names = decoded.PropertyInfos.Where(x => x.PropertyType.HasFlag(DtoPropertyTypes.ReadOnly)).Select(x => x.PropertyInfo.Name).ToArray();
            names.ShouldEqual(new []{ nameof(DddCompositeIntStringCreateDto.MyString), nameof(DddCompositeIntStringCreateDto.MyInt) });
        }

        //-----------------------------------------------------------
        //DecodedDto methods

        [Fact]
        public void TestFindSetterMethodWithGivenNumParams()
        {
            //SETUP
            var decoded = new DecodedDto(typeof(DddCompositeIntStringCreateDto), _EntityInfo, new GenericServicesConfig(), null);

            //ATTEMPT
            var methodOrCtor = decoded.GetCtorStaticCreatorToRun(new DecodeName(null), _EntityInfo);

            //VERIFY
            methodOrCtor.Name.ShouldEqual("Ctor");
        }
    }
}