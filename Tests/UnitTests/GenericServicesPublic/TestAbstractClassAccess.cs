// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    /// <summary>
    /// These tests are here because I had a problem with abstract classes. I haven't found anything wrong (so far!)
    /// </summary>
    public class TestAbstractClassAccess
    {
        //-------------------------------------------------------
        //Abstract base to entity

        [Fact]
        public void TestUpdateAbstractPropViaAutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TestAbstractMain(null,null,null));
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<TestAbstractUpdateViaAutoMapper>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new TestAbstractUpdateViaAutoMapper {Id = 1, AbstractPropPublic = "Test"};
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.TestAbstractMains.Single().AbstractPropPublic.ShouldEqual("Test");
            }
        }

        /// <summary>
        /// This method shows that it silently missed calling a method in a class that was inherited by the 
        /// </summary>
        [Fact]
        public void TestUpdateAbstractPropViaMethod_DOES_NOT_WORK()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new TestAbstractMain(null, null, null));
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<TestAbstractUpdateViaMethod>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new TestAbstractUpdateViaMethod { Id = 1, AbstractPropPrivate = "Test" };
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.TestAbstractMains.Single().AbstractPropPrivate.ShouldEqual(null);
            }
        }

        [Fact]
        public void TestCreateTestAbstractMainOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<TestAbstractCreate>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new TestAbstractCreate
                {
                    AbstractPropPrivate = "Base Private",
                    AbstractPropPublic = "Base Public",
                    NotAbstractPropPublic = "Main Public"
                };
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                dto.Id.ShouldEqual(1);
                context.TestAbstractMains.Single().AbstractPropPrivate.ShouldEqual("Base Private");
                context.TestAbstractMains.Single().AbstractPropPublic.ShouldEqual("Base Public");
                context.TestAbstractMains.Single().NotAbstractPropPublic.ShouldEqual("Main Public");
            }
        }

        //---------------------------------------------------------------------
        //abstract DTO

        [Fact]
        public void TestCreateViaDtoWithAbstractBaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<DddCtorEntityAbstractMainDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new DddCtorEntityAbstractMainDto()
                {
                    MyString = "Test",
                    MyInt = 123
                };
                service.CreateAndSave(dto, "ctor(2)");

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.DddCtorEntities.Single().Id.ShouldEqual(1);
                context.DddCtorEntities.Single().MyString.ShouldEqual("Test");
                context.DddCtorEntities.Single().MyInt.ShouldEqual(123);
            }
        }

    }
}