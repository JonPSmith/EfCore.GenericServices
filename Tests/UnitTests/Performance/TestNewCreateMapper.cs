// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using GenericServices.Setup;
using Tests.Dtos;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Performance
{
    public class TestNewCreateMapper
    {
        private readonly ITestOutputHelper _output;

        public TestNewCreateMapper(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCreateMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            var context = new EfCoreContext(options);
            var wrapperMapperConfigs = context.SetupSingleDtoAndEntities<BookTitleAndCount>();
            var entityInfo = typeof(Book).GetRegisteredEntityInfo();

            var myGeneric = typeof(CreateMapper.GenericMapper<,>);
            var genericType = myGeneric.MakeGenericType(typeof(BookTitleAndCount), entityInfo.EntityType);
            //ATTEMPT
            using (new TimeThings(_output, "Activator.CreateInstance", 100))
            {             
                for (int i = 0; i < 100; i++)
                {
                    dynamic instance = Activator.CreateInstance(genericType, context, wrapperMapperConfigs, entityInfo);
                    ((string)instance.EntityName).ShouldEqual("Book");
                }
            }
            var constructor = genericType.GetConstructors().Single();
            using (new TimeThings(_output, "1 x Create Linq in cache"))
            {
                CreateMapper.GetNewGenericMapper(genericType, constructor);
            }

            using (new TimeThings(_output, "LINQ new (cached)", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    dynamic instance = CreateMapper.GetNewGenericMapper(genericType, constructor).Invoke(context, wrapperMapperConfigs, entityInfo);
                    ((string)instance.EntityName).ShouldEqual("Book");
                }
            }
            using (new TimeThings(_output, "LINQ new (no cache)", 100))
            {
                for (int i = 0; i < 100; i++)
                {
                    dynamic instance = CreateMapper.NewGenericMapper(constructor).Invoke(context, wrapperMapperConfigs, entityInfo);
                    ((string)instance.EntityName).ShouldEqual("Book");
                }
            }
            //VERIFY
            true.ShouldBeTrue();
        }
    }
}