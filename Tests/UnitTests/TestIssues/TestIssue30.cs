// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericServices;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue30
    {
        [Fact]
        public void Dto_with_no_properties_causes_null_exception()
        {
            var options = SqliteInMemory.CreateOptions<SomeContext>();
            using (var context = new SomeContext(options))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<ImutableDto>());

                //VERIFY
                ex.Message.ShouldEqual("The ImutableDto class inherits ILinkToEntity<T> but has no properties in it!");
            }
        }


        public class SomeContext : DbContext
        {
            public SomeContext(DbContextOptions<SomeContext> options) : base(options) { }
            public DbSet<ImutableEntity> Entities { get; set; }

            protected override void OnModelCreating(ModelBuilder builder)
            {
                builder.Entity<ImutableEntity>().Property(e => e.Id).ValueGeneratedNever();
                builder.Entity<ImutableEntity>().Property(e => e.Name);
            }
        }

        public class ImutableEntity
        {
            public ImutableEntity(int id, string name)
            {
                Id = id;
                Name = name;
            }

            public int Id { get; private set; }
            public string Name { get; private set; }
        }

        public class ImutableDto : ILinkToEntity<ImutableEntity>
        {}
    }
}