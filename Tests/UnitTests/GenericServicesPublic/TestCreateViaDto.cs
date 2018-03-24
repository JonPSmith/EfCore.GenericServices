// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestCreateViaDto
    {
        public class AuthorDto : ILinkToEntity<Author>
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        [Fact]
        public void TestCreateAuthorViaAutoMapperOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var wrapped = context.SetupSingleDtoAndEntities<AuthorDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var author = new AuthorDto { Name = "New Name", Email = unique };
                service.AddNewAndSave(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestCreateAuthorViaAutoMapperMappingViaTestSetupOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var wrapped = context.SetupSingleDtoAndEntities<AuthorDto>(true);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var author = new AuthorDto { Name = "New Name", Email = unique };
                service.AddNewAndSave(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestCreateEntityUsingDefaults_AutoMapperOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var mapper = AutoMapperHelpers.CreateWrapperMapper<AuthorNameDto, Author>();
                context.SetupSingleDtoAndEntities<AuthorNameDto>(false);
                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                service.AddNewAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Single().Name.ShouldEqual("New Name");
                context.Authors.Single().Email.ShouldBeNull();
            }
        }

        [Fact]
        public void TestCreateEntityUsingDefaults_AutoMapperMappingSetOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new EfCoreContext(options))
            {
                var mapper = context.SetupSingleDtoAndEntities<AuthorNameDto>(true);
                var service = new GenericService<EfCoreContext>(context, mapper);

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                service.AddNewAndSave(dto, "AutoMapper");

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
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

        [Fact]
        public void TestCreateEntityUsingCtorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var mapper = context.SetupSingleDtoAndEntities<DtoCtorCreate>(true);
                var service = new GenericService<TestDbContext>(context, mapper);

                //ATTEMPT
                var dto = new DtoCtorCreate { MyInt = 1, MyString = "Hello"};
                service.AddNewAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.DddCtorEntities.Count().ShouldEqual(1);
                context.DddCtorEntities.Single().MyString.ShouldEqual("Hello");
                context.DddCtorEntities.Single().MyInt.ShouldEqual(1);
            }
        }

        public class DtoStaticFactoryCreate : ILinkToEntity<DddStaticFactEntity>
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
        }

        [Fact]
        public void TestCreateEntityUsingStaticFactoryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var mapper = context.SetupSingleDtoAndEntities<DtoStaticFactoryCreate>(true);
                var service = new GenericService<TestDbContext>(context, mapper);

                //ATTEMPT
                var dto = new DtoStaticFactoryCreate { MyInt = 1, MyString = "Hello"};
                service.AddNewAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
            using (var context = new TestDbContext(options))
            {
                context.DddStaticFactEntities.Count().ShouldEqual(1);
                context.DddStaticFactEntities.Single().MyString.ShouldEqual("Hello");
                context.DddStaticFactEntities.Single().MyInt.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateEntityUsingStaticFactoryWithBadStatusOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var mapper = context.SetupSingleDtoAndEntities<DtoStaticFactoryCreate>(true);
                var service = new GenericService<TestDbContext>(context, mapper);

                //ATTEMPT
                var dto = new DtoStaticFactoryCreate { MyInt = 1, MyString = null };
                service.AddNewAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("AddNew: The string should not be null.");
                context.DddStaticFactEntities.Count().ShouldEqual(0);
            }
        }
    }
}