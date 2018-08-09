// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
        public void TestCreateWithValidationOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new NormalEntity {MyInt = 1};

                //ATTEMPT
                context.Add(entity);
                context.SaveChangesWithValidation();
            }
            using (var context = new TestDbContext(options))
            {
                //VERIFY
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestUpdateWithValidationOk()
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
                entity.MyInt = 2;
                context.SaveChangesWithValidation();
            }
            using (var context = new TestDbContext(options))
            {
                //VERIFY
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(2);
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
                await context.SaveChangesWithValidationAsync();
            }
            using (var context = new TestDbContext(options))
            {
                //VERIFY
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
                var entity = new NormalEntity { MyInt = 1 };
                context.Add(entity);
                await context.SaveChangesWithValidationAsync();
            }
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                context.NormalEntities.Count().ShouldEqual(1);
                var entity = context.NormalEntities.Single();
                entity.MyInt = 2;
                await context.SaveChangesWithValidationAsync();
            }
            using (var context = new TestDbContext(options))
            {
                //VERIFY
                context.NormalEntities.Count().ShouldEqual(1);
                context.NormalEntities.Single().MyInt.ShouldEqual(2);
            }
        }

    }

}