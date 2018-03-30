// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.Startup;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestUnitTestSetup
    {
        public class DtoWithTwoIlinks : ILinkToEntity<Book>, ILinkToEntity<Author> {}

        public class DtoWithTwoConfigs : ILinkToEntity<Book> { }
        public class Congfig1 : PerDtoConfig<DtoWithTwoConfigs, Book> {}
        public class Congfig2 : PerDtoConfig<DtoWithTwoConfigs, Book> { }

        public class DtoWithoutILink { }

        [Fact]
        public void TestDtoWithTwoILinksBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<DtoWithTwoIlinks>());

                //VERIFY
                ex.Message.ShouldEqual("You had multiple ILinkToEntity interfaces on the DTO/VM DtoWithTwoIlinks. That isn't allowed.");
            }
        }

        [Fact]
        public void TestDtoWithoutLinkBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<DtoWithoutILink>());

                //VERIFY
                ex.Message.ShouldEndWith("The DTO/ViewModel class DtoWithoutILink is not registered as a valid GenericService DTO. Have you left off the ILinkToEntity interface?");
            }
        }

        [Fact]
        public void TestDtoWithTwoPerDtoConfigsDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<DtoWithTwoConfigs>());

                //VERIFY
                ex.Message.ShouldEndWith("I found multiple classes based on PerDtoConfig<DtoWithTwoConfigs,Book>, but you are only allowed one. They are: Congfig1, Congfig2.");
            }
        }


    }
}