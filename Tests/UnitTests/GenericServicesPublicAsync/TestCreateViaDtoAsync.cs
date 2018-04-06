// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestCreateViaDtoAsync
    {
        public class AuthorDto : ILinkToEntity<Author>
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        [Fact]
        public async Task TestCreateAuthorViaAutoMapperOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var wrapped = context.SetupSingleDtoAndEntities<AuthorDto>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var author = new AuthorDto { Name = "New Name", Email = unique };
                await service.AddNewAndSaveAsync(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public async Task TestCreateAuthorViaAutoMapperMappingViaTestSetupOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var wrapped = context.SetupSingleDtoAndEntities<AuthorDto>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var author = new AuthorDto { Name = "New Name", Email = unique };
                await service.AddNewAndSaveAsync(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public async Task TestCreateEntityUsingDefaults_AutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {              
                var wrapped = context.SetupSingleDtoAndEntities<AuthorNameDto>();
                context.SetupSingleDtoAndEntities<AuthorNameDto>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                await service.AddNewAndSaveAsync(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Single().Name.ShouldEqual("New Name");
                context.Authors.Single().Email.ShouldBeNull();
            }
        }

        [Fact]
        public async Task TestCreateEntityUsingDefaults_AutoMapperMappingSetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var wrapped = context.SetupSingleDtoAndEntities<AuthorNameDto>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                await service.AddNewAndSaveAsync(dto, CrudValues.UseAutoMapper);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Single().Name.ShouldEqual("New Name");
                context.Authors.Single().Email.ShouldBeNull();
            }
        }

        //------------------------------------------------------
        //via ctors/statics

        public class DtoCtorCreate : ILinkToEntity<DddCtorEntity>
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
        }

        public class DtoStaticCreate : ILinkToEntity<DddStaticCreateEntity>
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
        }

        [Fact]
        public async Task TestCreateEntityUsingStaticCreateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var wrapped = context.SetupSingleDtoAndEntities<DtoStaticCreate>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var dto = new DtoStaticCreate { MyInt = 1, MyString = "Hello"};
                await service.AddNewAndSaveAsync(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Ddd Static Create Entity");
            }
            using (var context = new TestDbContext(options))
            {
                context.DddStaticFactEntities.Count().ShouldEqual(1);
                context.DddStaticFactEntities.Single().MyString.ShouldEqual("Hello");
                context.DddStaticFactEntities.Single().MyInt.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestCreateEntityUsingStaticCreateWithBadStatusOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var wrapped = context.SetupSingleDtoAndEntities<DtoStaticCreate>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var dto = new DtoStaticCreate { MyInt = 1, MyString = null };
                await service.AddNewAndSaveAsync(dto);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("The string should not be null.");
                context.DddStaticFactEntities.Count().ShouldEqual(0);
            }
        }
    }
}