// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using Tests.UnitTests.GenericServicesInternal;
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
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var bookSeeded = context.SeedDatabaseFourBooks();

                var utData = context.SetupSingleDtoAndEntities<AddReviewWithIncludeDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new AddReviewWithIncludeDto
                    {BookId = bookSeeded.Last().BookId, Comment = "bad", NumStars = 1, VoterName = "user"};
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var entity = context.Books.Include(x => x.Reviews).Single(x => x.BookId == bookSeeded.Last().BookId);
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

                var utData = context.SetupSingleDtoAndEntities<UpdateBookWithAuthorUsingIncludeDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new UpdateBookWithAuthorUsingIncludeDto
                {
                    BookId = bookSeeded.First().BookId, 
                    AddThisAuthor = bookSeeded.Last().AuthorsLink.First().Author,
                    Order = 2
                };
                service.UpdateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                service.Message.ShouldEqual("Successfully updated the Book");
                var bookAuthorsName = context.Books
                    .Where(x => x.BookId == bookSeeded.First().BookId)
                    .SelectMany(x => x.AuthorsLink.Select(y => y.Author.Name))
                    .ToArray();
                bookAuthorsName.ShouldEqual(new String[] { "Martin Fowler", "Future Person" });
            }
        }
    }
}