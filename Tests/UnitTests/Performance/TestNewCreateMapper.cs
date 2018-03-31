// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using GenericServices.Startup;
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
            using (new TimeThings(_output, "Activator.CreateInstance"))
            {             
                for (int i = 0; i < 100; i++)
                {
                    Activator.CreateInstance(genericType, context, wrapperMapperConfigs, entityInfo);
                }
            }
            var constructor = genericType.GetConstructors().Single();
            using (new TimeThings(_output, "Create Linq"))
            {
                CreateMapper.GetNewGenericMapper(genericType, constructor);
            }

            using (new TimeThings(_output, "LINQ new (cached)"))
            {
                for (int i = 0; i < 100; i++)
                {
                    CreateMapper.GetNewGenericMapper(genericType, constructor).Invoke(context, wrapperMapperConfigs, entityInfo);
                }
            }
            using (new TimeThings(_output, "LINQ new (no cache)"))
            {
                for (int i = 0; i < 100; i++)
                {
                    CreateMapper.NewGenericMapper(constructor).Invoke(context, wrapperMapperConfigs, entityInfo);
                }
            }
            //VERIFY
            true.ShouldBeTrue();
        }
    }
}