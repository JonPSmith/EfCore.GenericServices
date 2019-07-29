// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using GenericServices;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue30
    {

        [Fact]
        public void Dto_with_no_properties_causes_null_exception()
        {
            using (var context = new SomeContext(InMemoryOptions.Create<SomeContext>()))
            {
                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<ImutableDto>());

                //VERIFY
                ex.Message.ShouldEqual("A DTO using the ILinkToEntity<T> must contain at least one Property!");
            }
        }

        private static class InMemoryOptions
        {
            public static DbContextOptions<T> Create<T>(string databaseName = "InMemory") where T : DbContext
                => new DbContextOptionsBuilder<T>()
                    .UseInMemoryDatabase(databaseName: databaseName)
                    .Options;
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
        {

        }
    }
}