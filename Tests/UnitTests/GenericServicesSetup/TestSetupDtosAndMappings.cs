// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.Startup;
using GenericServices.Startup.Internal;
using ServiceLayer.HomeController.Dtos;
using Tests.Dtos;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestSetupDtosAndMappings
    {

        [Fact]
        public void TestSetupSingleDtoAndEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.RegisterEntityClasses();

                //ATTEMPT
                var setupDtos = new SetupDtosAndMappings(new GenericServicesConfig());
                var wrappedMappings = setupDtos.ScanAllAssemblies(new[] {typeof(BookListDto).Assembly}, false);

                //VERIFY
                setupDtos.IsValid.ShouldBeTrue(setupDtos.GetAllErrors());
                wrappedMappings.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestSetupSingleDtoAndEntitiesInitializeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.RegisterEntityClasses();

                //ATTEMPT
                var setupDtos = new SetupDtosAndMappings(new GenericServicesConfig());
                var wrappedMappings = setupDtos.ScanAllAssemblies(new[] { typeof(BookListDto).Assembly }, true);

                //VERIFY
                setupDtos.IsValid.ShouldBeTrue(setupDtos.GetAllErrors());
                wrappedMappings.ShouldNotBeNull();
            }
        }


    }
}