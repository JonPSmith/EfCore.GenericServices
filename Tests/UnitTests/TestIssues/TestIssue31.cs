// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue31
    {

        [Fact]
        public async Task Root_method_call_fails()
        {
            // arrange
            var options = SqliteInMemory.CreateOptions<RootContext>();
            using (var context = new RootContext(options))
            {
                context.Database.EnsureCreated();
                var utData = context.SetupSingleDtoAndEntities<SetItemDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                // act
                var dto = new SetItemDto
                {
                    Id = 1,
                    Item = "A item!"
                };
                await service.UpdateAndSaveAsync(dto); 
            }
        }

        public class SetItemDto : ILinkToEntity<Root>
        {
            [ReadOnly(true)]
            public int Id { get; set; }
            public string Item { get; set; }
        }

        public class Root
        {
            public int Id { get; set; }
            public void SetItem(string item, RootContext context = null)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));
                // This method is never called
            }
        }

        public class RootContext : DbContext
        {
            public RootContext(DbContextOptions<RootContext> options) : base(options) { }
            protected override void OnModelCreating(ModelBuilder builder)
            {
                builder.Entity<Root>().Property(e => e.Id).ValueGeneratedNever();
                builder.Entity<Root>().HasData(new Root { Id = 1 });
            }
        }
    }
}