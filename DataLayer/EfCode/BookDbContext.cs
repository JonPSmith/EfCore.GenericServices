// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class BookDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        public BookDbContext(
            DbContextOptions<BookDbContext> options)
            : base(options)
        {
        }

        protected override void
            OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
            modelBuilder.ApplyConfiguration(new BookAuthorConfig());
        }
    }
}