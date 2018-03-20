// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Startup;
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
                var wrapped = new WrappedAutoMapperConfig( AutoMapperHelpers.CreateConfig<AuthorDto, Author>());
                context.SetupSingleDtoAndEntities<AuthorDto>(false);
                var service = new GenericService<EfCoreContext>(context, wrapped);

                //ATTEMPT
                var author = new AuthorDto { Name = "New Name", Email = unique };
                service.Create(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(string.Join("\n", service.Errors));
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
                service.Create(author);

                //VERIFY
                service.IsValid.ShouldBeTrue(string.Join("\n", service.Errors));
            }
            using (var context = new EfCoreContext(options))
            {
                context.Authors.Count().ShouldEqual(1);
                context.Authors.Find(1).Email.ShouldEqual(unique);
            }
        }

    }
}