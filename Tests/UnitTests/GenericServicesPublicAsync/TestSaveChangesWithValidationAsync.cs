// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublicAsync
{
    public class TestSaveChangesWithValidationAsync
    {
        public class LocalAuthorDto : ILinkToEntity<Author>
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class LocalAuthorDtoWithConfig : ILinkToEntity<Author>
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class CongfigWithValidation : PerDtoConfig<LocalAuthorDtoWithConfig, Author>
        {
            public override bool? UseSaveChangesWithValidation => true;
        }

        [Fact]
        public async Task TestCreateAuthorNameGoodNoValidationOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var wrapped = context.SetupSingleDtoAndEntities<LocalAuthorDto>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var author = new LocalAuthorDto { Name = "Name", Email = unique };
                await service.CreateAndSaveAsync(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
            }
        }

        [Fact]
        public async Task TestCreateAuthorNameNullNoValidationOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var wrapped = context.SetupSingleDtoAndEntities<LocalAuthorDto>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var author = new LocalAuthorDto { Name = null, Email = unique };
                var ex = await Assert.ThrowsAsync<Microsoft.EntityFrameworkCore.DbUpdateException> (() => service.CreateAndSaveAsync(author));

                //VERIFY
                ex.InnerException?.Message.ShouldEqual("SQLite Error 19: 'NOT NULL constraint failed: Authors.Name'.");
            }
        }

        [Fact]
        public async Task TestCreateAuthorNameNullPerDtoConfigValidationOk()
        {
            //SETUP
            var unique = Guid.NewGuid().ToString();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var wrapped = context.SetupSingleDtoAndEntities<LocalAuthorDtoWithConfig>();
                var service = new CrudServicesAsync(context, wrapped);

                //ATTEMPT
                var author = new LocalAuthorDtoWithConfig { Name = null, Email = unique };
                await service.CreateAndSaveAsync(author);

                //VERIFY
                service.IsValid.ShouldBeFalse();
                service.GetAllErrors().ShouldEqual("Author: The Name field is required.");
            }
        }


    }
}