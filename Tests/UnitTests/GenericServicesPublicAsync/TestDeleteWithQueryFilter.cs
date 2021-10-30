// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestDeleteWithQueryFilter
    {
        private  Task<IStatusGeneric> DelHandlerAsync(DbContext context, SoftDelEntity entity)
        {
            var status = new StatusGenericHandler();
            if (!entity.SoftDeleted)
                status.AddError("Can't delete if not already soft deleted.");
            return Task.FromResult((IStatusGeneric)status);
        }

        [Fact]
        public async Task TestDeleteAsyncWithQueryFilterFailsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var author = new SoftDelEntity { SoftDeleted = true };
                context.Add(author);
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                context.SoftDelEntities.Count().ShouldEqual(0);
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(1);

                //ATTEMPT
                await service.DeleteAndSaveAsync<SoftDelEntity>(1);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Sorry, I could not find the Soft Del Entity you wanted to delete.");

                context.ChangeTracker.Clear();
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestDeleteWithActionAsyncWithQueryFilterOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var author = new SoftDelEntity { SoftDeleted = true };
                context.Add(author);
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                context.SoftDelEntities.Count().ShouldEqual(0);
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(1);

                //ATTEMPT
                await service.DeleteWithActionAndSaveAsync<SoftDelEntity>(DelHandlerAsync, 1);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully deleted a Soft Del Entity");

                context.ChangeTracker.Clear();
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public async  Task TestDeleteWithActionAsyncWithQueryFilterError()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var author = new SoftDelEntity { SoftDeleted = false };
                context.Add(author);
                context.SaveChanges();

                context.ChangeTracker.Clear();

                var utData = context.SetupEntitiesDirect();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                context.SoftDelEntities.Count().ShouldEqual(1);
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(1);

                //ATTEMPT
                await service.DeleteWithActionAndSaveAsync<SoftDelEntity>(DelHandlerAsync, 1);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Can't delete if not already soft deleted.");

                context.ChangeTracker.Clear();
                context.SoftDelEntities.IgnoreQueryFilters().Count().ShouldEqual(1);
            }
        }
    }
}