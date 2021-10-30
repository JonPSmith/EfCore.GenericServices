// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestUpdateWithIncludeThenAttribute
    {
        [Fact]
        public void TestCallAddReviewWithIncludeWithIncludedReviews()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var bookSeeded = context.SeedDatabaseFourBooks();
                bookId = bookSeeded.Last().BookId;

                context.ChangeTracker.Clear();

                var utData = context.SetupSingleDtoAndEntities<AddReviewWithIncludeDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new AddReviewWithIncludeDto
                    {BookId = bookId, Comment = "bad", NumStars = 1, VoterName = "user"};
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookId);
                entity.Reviews.Count().ShouldEqual(3);
                entity.Reviews.Single(x => x.Comment == "bad").NumStars.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCallUpdateBookWithExistingAuthorWithTwoIncludes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var bookSeeded = context.SeedDatabaseFourBooks();
                var bookId = bookSeeded.First().BookId;
                var authorId = bookSeeded.Last().AuthorsLink.First().AuthorId;

                context.ChangeTracker.Clear();

                var utData = context.SetupSingleDtoAndEntities<AddNewAuthorToBookUsingIncludesDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new AddNewAuthorToBookUsingIncludesDto
                {
                    BookId = bookId,
                    AddThisAuthor = context.Authors.SingleOrDefault(x => x.AuthorId == authorId),
                    Order = 2
                };
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var bookAuthorsName = context.Books
                    .Where(x => x.BookId == bookId)
                    .SelectMany(x => x.AuthorsLink.Select(y => y.Author.Name))
                    .ToArray();
                bookAuthorsName.ShouldEqual(new string[] {"Martin Fowler", "Future Person"});
            }
        }

        //-----------------------------------------------
        //async versions

        [Fact]
        public async Task TestCallAddReviewWithIncludeWithIncludedReviewsAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var bookSeeded = context.SeedDatabaseFourBooks();
                var bookId = bookSeeded.Last().BookId;

                context.ChangeTracker.Clear();

                var utData = context.SetupSingleDtoAndEntities<AddReviewWithIncludeDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new AddReviewWithIncludeDto
                    {BookId = bookId, Comment = "bad", NumStars = 1, VoterName = "user"};
                await service.UpdateAndSaveAsync(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookId);
                entity.Reviews.Count().ShouldEqual(3);
                entity.Reviews.Single(x => x.Comment == "bad").NumStars.ShouldEqual(1);
            }
        }

        [Fact]
        public async Task TestCallUpdateBookWithExistingAuthorWithTwoIncludesAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var bookSeeded = context.SeedDatabaseFourBooks();
                var bookId = bookSeeded.First().BookId;
                var authorId = bookSeeded.Last().AuthorsLink.First().AuthorId;

                context.ChangeTracker.Clear();

                var utData = context.SetupSingleDtoAndEntities<AddNewAuthorToBookUsingIncludesDto>();
                var service = new CrudServicesAsync(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new AddNewAuthorToBookUsingIncludesDto
                {
                    BookId = bookId,
                    AddThisAuthor = context.Authors.SingleOrDefault(x => x.AuthorId == authorId),
                    Order = 2
                };
                await service.UpdateAndSaveAsync(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var bookAuthorsName = context.Books
                    .Where(x => x.BookId == bookId)
                    .SelectMany(x => x.AuthorsLink.Select(y => y.Author.Name))
                    .ToArray();
                bookAuthorsName.ShouldEqual(new string[] {"Martin Fowler", "Future Person"});
            }
        }
    }
}