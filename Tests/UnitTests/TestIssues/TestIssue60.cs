// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue60
    {
        public class entity
        {
            [Key]
            public long Id { get; set; }
            public Reference Object { get; set; }
            public Reference Object2 { get; set; }
        }

        [Owned]
        public class Reference
        {
            [StringLength(50)]
            public string Type { get; set; }
        }

        public class Context : DbContext
        {
            public Context(DbContextOptions<Context> options)
                : base(options)
            {
            }

            public DbSet<TestIssue60.entity> Entities { get; set; }
        }

        [Fact]
        public void Test60()
        {
            var options = SqliteInMemory.CreateOptions<Context>();
            using (var context = new Context(options))
            {
                context.Database.EnsureCreated();
                var utData = context.SetupEntitiesDirect();

                var service = new CrudServices(context, utData.ConfigAndMapper);
            }
        }


        [Fact]
        public void TestGetEntityTypes()
        {
            var options = SqliteInMemory.CreateOptions<Context>();
            using var context = new Context(options);

            //ATTEMPT
            var ownedTypes = context.Model.GetEntityTypes()
                                          .Where(x => x.ClrType.Name == typeof(Reference).Name);

            //VERIFY
            ownedTypes.Count().ShouldEqual(2);
            ownedTypes.Select(x => x.ClrType.ShouldBeType<Reference>());
        }

    }
}