// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.Setup;
using Tests.Dtos;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestUnitTestSetup
    {
        [Fact]
        public void TestSetupSingleDtoAndEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var utData = context.SetupSingleDtoAndEntities<BookTitle>();

                //VERIFY
                utData.ConfigAndMapper.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestSetupTwoDtoAndEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var utData = context.SetupSingleDtoAndEntities<BookTitle>();
                utData.AddSingleDto<BookTitleAndCount>();

                //VERIFY
                utData.ConfigAndMapper.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestSetupDtoWithTwoILinksBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<DtoWithTwoIlinks>());

                //VERIFY
                ex.Message.ShouldEndWith("You had multiple ILinkToEntity interfaces on the DTO/VM DtoWithTwoIlinks. That isn't allowed.");
            }
        }

        [Fact]
        public void TestSetupDtoWithoutLinkBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<DtoWithoutILink>());

                //VERIFY
                ex.Message.ShouldEndWith("The class DtoWithoutILink is not registered as a valid CrudServices DTO/ViewModel. Have you left off the ILinkToEntity interface?");
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

        [Fact]
        public void TestSetupPrivateDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<PrivateDto>());

                //VERIFY
                ex.Message.ShouldEndWith("PrivateDto: Sorry, but the DTO/ViewModel class 'PrivateDto' must be public for GenericServices to work.");
            }
        }

        public class DtoWithTwoIlinks : ILinkToEntity<Book>, ILinkToEntity<Author> {}

        public class DtoWithTwoConfigs : ILinkToEntity<Book> {}

        public class Congfig1 : PerDtoConfig<DtoWithTwoConfigs, Book> {}

        public class Congfig2 : PerDtoConfig<DtoWithTwoConfigs, Book> {}

        public class DtoWithoutILink {}

        private class PrivateDto : ILinkToEntity<Book> {}
    }
}