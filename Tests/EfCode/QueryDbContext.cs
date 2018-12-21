// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;
using Tests.EfClasses;

namespace Tests.EfCode
{
    public class QueryDbContext : DbContext
    {
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Child> WritableChildren { get; set; }
        public DbQuery<ChildReadOnly> Children { get; set; }

        public QueryDbContext(
            DbContextOptions<QueryDbContext> options)
            : base(options)
        {
        }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Child>().ToTable("Children");

            //modelBuilder.Query<ParentWithChildCountQuery>().ToView("Children");
        }
    }
}