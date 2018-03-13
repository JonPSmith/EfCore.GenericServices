using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericLibsBase;
using Xunit;
using GenericServices.Internal.Decoders;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServices
{
    public class TestDecodedEntityClass
    {


        [Fact]
        public void AuthorDecodedEntityClass()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var status = DecodedEntityClass.CreateFactory(typeof(Author), context);

                //VERIFY
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors));
                status.Result.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Author.AuthorId));
                status.Result.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeFalse();
                status.Result.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeTrue();
                status.Result.EntityClassInfo.CanBeCreated.ShouldBeTrue();
                status.Result.EntityClassInfo.PropertiesWithPublicSetter.Select(x => x.Name)
                    .ShouldEqual(new []{nameof(Author.AuthorId), nameof(Author.Name), nameof(Author.Email), nameof(Author.BooksLink) });
            }
        }

        [Fact]
        public void ReviewDecodedEntityClass()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var status = DecodedEntityClass.CreateFactory(typeof(Review), context);

                //VERIFY
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors));
                status.Result.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Review.ReviewId));
                status.Result.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeFalse();
                status.Result.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeFalse();
                status.Result.EntityClassInfo.CanBeCreated.ShouldBeFalse();
                status.Result.EntityClassInfo.PublicCtors.Length.ShouldEqual(0);
            }
        }

        [Fact]
        public void BookDecodedEntityClass()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var status = DecodedEntityClass.CreateFactory(typeof(Book), context);

                //VERIFY
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors));
                status.Result.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Book.BookId));
                status.Result.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeTrue();
                status.Result.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeFalse();
                status.Result.EntityClassInfo.CanBeCreated.ShouldBeTrue();
                status.Result.EntityClassInfo.PublicCtors.Length.ShouldEqual(1);
                status.Result.EntityClassInfo.PublicStaticFactoryMethods.Length.ShouldEqual(0);
                status.Result.EntityClassInfo.PublicSetterMethods.Length.ShouldEqual(5);
            }
        }

        [Fact]
        public void OrderDecodedEntityClass()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var status = DecodedEntityClass.CreateFactory(typeof(Order), context);

                //VERIFY
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors));
                status.Result.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Order.OrderId));
                status.Result.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeTrue();
                status.Result.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeFalse();
                status.Result.EntityClassInfo.CanBeCreated.ShouldBeTrue();
                status.Result.EntityClassInfo.PublicCtors.Length.ShouldEqual(0);
                status.Result.EntityClassInfo.PublicStaticFactoryMethods.Length.ShouldEqual(1);
                status.Result.EntityClassInfo.PublicSetterMethods.Length.ShouldEqual(1);
            }
        }

    }
}