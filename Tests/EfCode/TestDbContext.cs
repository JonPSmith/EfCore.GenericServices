// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Tests.EfClasses;

namespace Tests.EfCode
{
    public class TestDbContext : DbContext
    {
        public DbSet<NormalEntity> NormalEntities { get; set; }
        public DbSet<DddCtorEntity> DddCtorEntities { get; set; }
        public DbSet<DddStaticFactEntity> DddStaticFactEntities { get; set; }
        public DbSet<DddCtorAndFactEntity> DddCtorAndFactEntities { get; set; }
        public DbSet<NotUpdatableEntity> NotUpdatableEntities { get; set; }
        public DbSet<ReadOnlyEntity> ReadOnlyEntities { get; set; }

        public TestDbContext(DbContextOptions<TestDbContext> options): base(options) { }
    }
}