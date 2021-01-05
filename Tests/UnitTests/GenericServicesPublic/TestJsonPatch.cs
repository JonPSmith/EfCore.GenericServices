// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.AspNetCore.JsonPatch;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestJsonPatch
    {
        [Fact]
        public void TestUpdateJsonPatchKeysOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var patch = new JsonPatchDocument<Author>();
                patch.Replace(x => x.Name, unique);
                service.UpdateAndSave(patch, 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Author");
                context.Authors.Find(1).Name.ShouldEqual(unique);
            }
        }

        [Fact]
        public void TestUpdateJsonPatchWhereOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var patch = new JsonPatchDocument<Author>();
                patch.Replace(x => x.Name, unique);
                service.UpdateAndSave(patch, x => x.AuthorId == 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Author");
                context.Authors.Find(1).Name.ShouldEqual(unique);
            }
        }

        //------------------------------------------------------------
        //errors


        [Fact]
        public void TestUpdateJsonPatchTestFailsOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var patch = new JsonPatchDocument<Book>();
                patch.Test(x => x.Title, "XXX");
                patch.Replace(x => x.Title, unique);
                service.UpdateAndSave(patch, 1);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldStartWith("The current value 'Refactoring' at path 'Title' is not equal to the test value 'XXX'.");
            }
        }

        [Fact]
        public void TestUpdateJsonPatchNoUpdateBecauseSetterIsPrivateOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var utData = context.SetupEntitiesDirect();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var patch = new JsonPatchDocument<Book>();
                patch.Replace(x => x.Title, unique);
                service.UpdateAndSave(patch, 1);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldStartWith("The property at path 'Title' could not be updated.");
            }
        }
    }
}