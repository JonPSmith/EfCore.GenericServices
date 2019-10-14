// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestDbQueryCrud
    {
        private readonly ITestOutputHelper _output;

        public TestDbQueryCrud(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestDbQueryReadManyDirectOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
#if NETCOREAPP3_0
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));
#endif
                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var entities = service.ReadManyNoTracked<ChildReadOnly>().ToList();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entities.Count.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestDbQueryReadManyViaDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
#if NETCOREAPP3_0
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));
#endif
                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<ChildDbQueryDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var entities = service.ReadManyNoTracked<ChildDbQueryDto>().ToList();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entities.Count.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestDbQueryReadSingleWhereDirectOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
#if NETCOREAPP3_0
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));
#endif
                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var entity = service.ReadSingle<ChildReadOnly>(x => x.ChildId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entity.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestDbQueryReadSingleWhereViaDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
#if NETCOREAPP3_0
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));
#endif
                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupSingleDtoAndEntities<ChildDbQueryDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var entity = service.ReadSingle<ChildDbQueryDto>(x => x.ChildId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entity.ShouldNotBeNull();
            }
        }

        //----------------------------------------------------------
        //error messages

        [Fact]
        public void TestDbQueryReadSingleFindFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => service.ReadSingle<ChildReadOnly>(1));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in a Find.");
            }
        }

        [Fact]
        public void TestDbQueryCreateFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() =>  service.CreateAndSave(new ChildReadOnly()));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Create.");
            }
        }

        [Fact]
        public void TestDbQueryUpdateFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateAndSave(new ChildReadOnly()));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Update.");
            }
        }

        [Fact] public void TestDbQueryDeleteFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteAndSave<ChildReadOnly>(1));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Delete.");
            }
        }

        [Fact]
        public void TestDbQueryDeleteWithRulesFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper, new CreateNewDBContextHelper(() => new TestDbContext(options)));

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => service.DeleteWithActionAndSave<ChildReadOnly>((c, e) => null, 1));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Delete.");
            }
        }
    }
}