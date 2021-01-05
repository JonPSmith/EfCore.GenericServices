// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesSetup
{
    public class TestBeforeSaveChangesSync
    {
        [Fact]
        public void TestNoBeforeSaveChangesMethodProvided()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.CreateAndSave(new NormalEntity {MyString = "bad word"});

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestBeforeSaveChangesMethodProvidedNoError()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericServicesConfig()
                {
                    BeforeSaveChanges = FailOnBadWord
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.CreateAndSave(new NormalEntity { MyString = "good word" });

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestBeforeSaveChangesMethodProvidedWithError()
        {
            //SETUP  
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var config = new GenericServicesConfig()
                {
                    BeforeSaveChanges = FailOnBadWord
                };
                var utData = context.SetupSingleDtoAndEntities<UniqueWithConfigDto>(config);
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                service.CreateAndSave(new NormalEntity { MyString = "bad word" });

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("The NormalEntity class contained a bad word.");
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(0);
            }
        }

        //-------------------------------------------------
        //BeforeSaveChanges test setup

        private IStatusGeneric FailOnBadWord(DbContext context)
        {
            var status = new StatusGenericHandler();
            var entriesToCheck = context.ChangeTracker.Entries()
                .Where(e =>
                    (e.State == EntityState.Added) ||
                    (e.State == EntityState.Modified));
            foreach (var entity in entriesToCheck)
            {
                if (entity.Entity is NormalEntity normalInstance && normalInstance.MyString.Contains("bad"))
                    status.AddError($"The {nameof(NormalEntity)} class contained a bad word.");
            }

            return status;
        }
    }
}