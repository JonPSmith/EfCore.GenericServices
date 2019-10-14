// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
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
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestCreateViaDto
    {
        private readonly ITestOutputHelper _output;

        public TestCreateViaDto(ITestOutputHelper output)
        {
            _output = output;
        }

        public class AuthorDto : ILinkToEntity<Author>
        {
            public int AuthorId { get; set; }
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

                var utData = context.SetupSingleDtoAndEntities<AuthorDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new EfCoreContext(options)));

                //ATTEMPT
                using (new TimeThings(_output, "CreateAndSave"))
                {
                    var author = new AuthorDto { Name = "New Name", Email = unique };
                    service.CreateAndSave(author);
                }

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
        public void TestCreateAuthorViaAutoMapperPrimaryKeyCopiedBackOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<AuthorDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new EfCoreContext(options)));

                //ATTEMPT
                var dto = new AuthorDto {Name = "New Name", Email = unique};
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Author");
                dto.AuthorId.ShouldNotEqual(0);
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
                var utData = context.SetupSingleDtoAndEntities<AuthorDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new EfCoreContext(options)));

                //ATTEMPT
                var author = new AuthorDto { Name = "New Name", Email = unique };
                service.CreateAndSave(author);

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
                var utData = context.SetupSingleDtoAndEntities<AuthorNameDto>();
                context.SetupSingleDtoAndEntities<AuthorNameDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new EfCoreContext(options)));

                //ATTEMPT
                var dto = new AuthorNameDto { Name = "New Name" };
                service.CreateAndSave(dto);

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
        public void TestCreateEntityNotCopyKeyBackBecauseDtoPropertyHasPrivateSetterOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<NormalEntityKeyPrivateSetDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new NormalEntityKeyPrivateSetDto();
                service.CreateAndSave(dto, CrudValues.UseAutoMapper);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                context.NormalEntities.Count().ShouldEqual(1);
                dto.Id.ShouldEqual(0);
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
        public void TestCreateEntityViaDtoCtorCreateCtor2Ok()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<DtoCtorCreate>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                using (new TimeThings(_output, "CreateAndSave"))
                {
                    var dto = new DtoCtorCreate { MyInt = 123, MyString = "Hello" };
                    service.CreateAndSave(dto, "ctor(2)");
                }

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Ddd Ctor Entity");
            }
            using (var context = new TestDbContext(options))
            {
                context.DddCtorEntities.Count().ShouldEqual(1);
                context.DddCtorEntities.Find(1).MyInt.ShouldEqual(123);
                context.DddCtorEntities.Find(1).MyString.ShouldEqual("Hello");
            }
        }

        [Fact]
        public void TestCreateEntityViaDtoCtorCreateCtor1Ok()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<DtoCtorCreate>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new DtoCtorCreate { MyInt = 123, MyString = "Hello" };
                service.CreateAndSave(dto, "ctor(1)");

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Ddd Ctor Entity");
            }
            using (var context = new TestDbContext(options))
            {
                context.DddCtorEntities.Count().ShouldEqual(1);
                context.DddCtorEntities.Find(1).MyInt.ShouldEqual(123);
                context.DddCtorEntities.Find(1).MyString.ShouldEqual("1 param ctor");
            }
        }

        [Fact]
        public void TestCreateEntityViaDtoCtorDddKeyIsString()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<DddCompositeIntStringCreateDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new DddCompositeIntStringCreateDto { MyString = "Hello", MyInt = 1 };
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully created a Ddd Composite Int String");
                context.DddCompositeIntStrings.Single().MyString.ShouldEqual("Hello");
                context.DddCompositeIntStrings.Single().MyInt.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateEntityViaDtoCtorCreateCtorMatchesLongerOne()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<DtoCtorCreate>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new DtoCtorCreate { MyInt = 123, MyString = "Hello" };
                service.CreateAndSave(dto);

                //VERIFY
                context.DddCtorEntities.Single().MyInt.ShouldEqual(123);
                context.DddCtorEntities.Single().MyString.ShouldEqual("Hello");
            }
        }

        public class DtoCtorCreateBad : ILinkToEntity<DddCtorEntity>
        {
            public int YourInt { get; set; }
            public bool MyString { get; set; }
        }

        [Fact]
        public void TestCreateEntityViaDtoCtorNoMatchOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<DtoCtorCreateBad>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new DtoCtorCreateBad { };
                var ex = Assert.Throws<InvalidOperationException>(() => service.CreateAndSave(dto));

                //VERIFY
                ex.Message.ShouldEqual(
                    "Could not find a ctor/static method that matches the DTO. The ctor/static method that fit the properties in the DTO/VM are:\n" +
                    "Matched 2 params out of 2. Score 50% Ctor(Name not match, but type is Match, wrong name)\n" +
                    "Matched 1 params out of 1. Score 30% Ctor(Name not match, but type is Match)");

            }
        }

        public class DtoStaticCreate : ILinkToEntity<DddStaticCreateEntity>
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
        }

        [Fact]
        public void TestCreateEntityUsingStaticCreatorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<DtoStaticCreate>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new DtoStaticCreate { MyInt = 1, MyString = "Hello"};
                service.CreateAndSave(dto);

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
        public void TestCreateEntityUsingStaticCreateWithBadStatusOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
            }
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<DtoStaticCreate>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var dto = new DtoStaticCreate { MyInt = 1, MyString = null };
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("The string should not be null.");
                context.DddStaticFactEntities.Count().ShouldEqual(0);
            }
        }
    }
}