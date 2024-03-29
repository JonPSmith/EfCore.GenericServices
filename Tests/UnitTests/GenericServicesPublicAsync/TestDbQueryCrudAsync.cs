﻿// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestDbQueryCrudAsync
    {
        private readonly ITestOutputHelper _output;

        public TestDbQueryCrudAsync(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task TestDbQueryReadManyDirectOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));

                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var entities = await service.ReadManyNoTracked<ChildReadOnly>().ToListAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entities.Count.ShouldEqual(2);
            }
        }

        [Fact]
        public async Task TestDbQueryReadManyViaDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));

                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupSingleDtoAndEntities<ChildDbQueryDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var entities = await service.ReadManyNoTracked<ChildDbQueryDto>().ToListAsync();

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entities.Count.ShouldEqual(2);
            }
        }

        [Fact]
        public async Task TestDbQueryReadSingleWhereDirectOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));

                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var entity = await service.ReadSingleAsync<ChildReadOnly>(x => x.ChildId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entity.ShouldNotBeNull();
            }
        }

        [Fact]
        public async Task TestDbQueryReadSingleWhereViaDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));

                context.Add(new Parent
                    { Children = new List<Child> { new Child { MyString = "Hello" }, new Child { MyString = "Goodbye" } } });
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupSingleDtoAndEntities<ChildDbQueryDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var entity = await service.ReadSingleAsync<ChildDbQueryDto>(x => x.ChildId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                entity.ShouldNotBeNull();
            }
        }

        //----------------------------------------------------------
        //error messages

        [Fact]
        public async Task TestDbQueryReadSingleFindFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ReadSingleAsync<ChildReadOnly>(1));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in a Find.");
            }
        }

        [Fact]
        public async Task TestDbQueryCreateFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>  service.CreateAndSaveAsync(new ChildReadOnly()));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Create.");
            }
        }

        [Fact]
        public async Task TestDbQueryUpdateFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateAndSaveAsync(new ChildReadOnly()));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Update.");
            }
        }

        [Fact]
        public async Task TestDbQueryDeleteFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.DeleteAndSaveAsync<ChildReadOnly>(1));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Delete.");
            }
        }

        [Fact]
        public async Task TestDbQueryDeleteWithRulesFail()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var ex = await Assert.ThrowsAsync<InvalidOperationException > (() => service.DeleteWithActionAndSaveAsync<ChildReadOnly>((c, e) => null, 1));

                //VERIFY
                ex.Message.ShouldEqual("The class ChildReadOnly of style HasNoKey cannot be used in Delete.");
            }
        }
    }
}