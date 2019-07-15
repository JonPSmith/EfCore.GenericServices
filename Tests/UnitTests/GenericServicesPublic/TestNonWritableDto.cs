using System;
using System.Linq;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.DataLayer
{
    public class TestNonWritableDto
    {
        [Fact]
        public void TestEmptyLinkedToImmutableEntityDto()
        {
            //SETUP
            InvalidOperationException exception;
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var utData = context.SetupSingleDtoAndEntities<EmptyLinkedToImmutableEntityDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);
                exception = Assert.Throws<InvalidOperationException>(() => service.CreateAndSave(new EmptyLinkedToImmutableEntityDto()));
            }

            //VERIFY
            exception.Message.ShouldStartWith("Could not find a ctor/static method that matches the DTO. The ctor/static method that fit the properties in the DTO/VM are:");
        }

        [Fact]
        public void TestEmptyLinkedToMutableEntityDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var utData = context.SetupSingleDtoAndEntities<EmptyLinkedToMutableEntityDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);
                service.CreateAndSave(new EmptyLinkedToMutableEntityDto());
            }

            //VERIFY
            using (var context = new TestDbContext(options))
            {
                var normalEntities = context.NormalEntities.ToList();
                normalEntities.Count.ShouldEqual(1);
                normalEntities[0].ShouldNotBeNull();
                normalEntities[0].Id.ShouldEqual(1);
                normalEntities[0].MyInt.ShouldEqual(0);
                normalEntities[0].MyString.ShouldBeNull();
            }
        }


        [Fact]
        public void TestReadonlyLinkedToImmutableEntityDto()
        {
            //SETUP
            InvalidOperationException exception;
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var utData = context.SetupSingleDtoAndEntities<ReadonlyLinkedToImmutableEntityDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);
                exception = Assert.Throws<InvalidOperationException>(() => service.CreateAndSave(new EmptyLinkedToImmutableEntityDto()));
            }

            //VERIFY
            exception.Message.ShouldStartWith("Could not find a ctor/static method that matches the DTO. The ctor/static method that fit the properties in the DTO/VM are:");
        }

        [Fact]
        public void TestReadonlyLinkedToMutableEntityDto()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var utData = context.SetupSingleDtoAndEntities<ReadonlyLinkedToMutableEntityDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);
                service.CreateAndSave(new ReadonlyLinkedToMutableEntityDto());
            }

            //VERIFY
            using (var context = new TestDbContext(options))
            {
                var normalEntities = context.NormalEntities.ToList();
                normalEntities.Count.ShouldEqual(1);
                normalEntities[0].ShouldNotBeNull();
                normalEntities[0].Id.ShouldEqual(1);
                normalEntities[0].MyInt.ShouldEqual(0);
                normalEntities[0].MyString.ShouldBeNull();
            }
        }
    }
}