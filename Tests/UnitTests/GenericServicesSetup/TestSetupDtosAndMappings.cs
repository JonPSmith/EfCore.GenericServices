// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfCode;
using GenericServices.Configuration;
using GenericServices.Setup.Internal;
using ServiceLayer.HomeController.Dtos;
using Tests.EfCode;
using TestSupport.Attributes;
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

        // This relies on only the EfCoreContext entities being registered
        [RunnableInDebugOnly]
        public void TestSetupSingleDtoAndEntitiesTestAssemblyEfCoreContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.RegisterEntityClasses();
            }

            //ATTEMPT
            var setupDtos = new SetupDtosAndMappings(new GenericServicesConfig());
            setupDtos.ScanAllAssemblies(new[] { GetType().Assembly }, true);

            //VERIFY
            setupDtos.IsValid.ShouldBeFalse();
            setupDtos.Errors.Count.ShouldEqual(4);
        }

        [Fact]
        public void TestSetupSingleDtoAndEntitiesTestAssemblyEfCoreContextAndTestDbContextOk()
        {
            //SETUP
            var options1 = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options1))
            {
                context.RegisterEntityClasses();
            }
            var options2 = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options2))
            {
                context.RegisterEntityClasses();
            }

            //ATTEMPT
            var setupDtos = new SetupDtosAndMappings(new GenericServicesConfig());
            setupDtos.ScanAllAssemblies(new[] { GetType().Assembly }, true);

            //VERIFY
            setupDtos.IsValid.ShouldBeFalse();
            setupDtos.Errors.Count.ShouldEqual(3);    
        }


    }
}