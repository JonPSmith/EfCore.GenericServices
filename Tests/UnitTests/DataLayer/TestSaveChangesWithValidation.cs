// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.DataLayer
{
    public class TestSaveChangesWithValidation
    {
        [Fact]
        public void TestSaveChangesWithValidationNoErrorsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new NormalEntity {MyInt = 1};

                //ATTEMPT
                context.Add(entity);
                var status = context.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldBeTrue(status.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestSaveChangesWithValidationWithErrorsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new NormalEntity { MyInt = 1 };
                context.Add(entity);
                context.SaveChangesWithValidation();
            }
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                context.NormalEntities.Count().ShouldEqual(1);
                var entity = context.NormalEntities.Single();
                entity.MyInt = 1000;
                var status = context.SaveChangesWithValidation();

                //VERIFY
                status.IsValid.ShouldBeFalse();
                status.GetAllErrors().ShouldEqual("Normal Entity: The field MyInt must be between 0 and 100.");
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(1);
            }

        }

        //------------------------------------------------------------
        //async

        [Fact]
        public async Task TestCrateWithValidationAsyncOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new NormalEntity { MyInt = 1 };

                //ATTEMPT
                context.Add(entity);
                var status = await context.SaveChangesWithValidationAsync();

                //VERIFY
                status.IsValid.ShouldBeTrue(status.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestUpdateWithValidationAsyncOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new NormalEntity {MyInt = 1};
                context.Add(entity);
                await context.SaveChangesWithValidationAsync();
            }

            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                context.NormalEntities.Count().ShouldEqual(1);
                var entity = context.NormalEntities.Single();
                entity.MyInt = 1000;
                var status = await context.SaveChangesWithValidationAsync();

                //VERIFY
                status.IsValid.ShouldBeFalse();
                status.GetAllErrors().ShouldEqual("Normal Entity: The field MyInt must be between 0 and 100.");
            }
            using (var context = new TestDbContext(options))
            {
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(1);
            }
        }
    }

}