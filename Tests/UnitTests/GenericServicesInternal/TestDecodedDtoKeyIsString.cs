using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using Xunit;
using GenericServices.Internal.Decoders;
using Microsoft.AspNetCore.Mvc;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.UnitTests.GenericServicesPublic;
using TestSupport.EfHelpers;
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