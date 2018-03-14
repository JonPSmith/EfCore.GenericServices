using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericLibsBase;
using Xunit;
using GenericServices.Internal.Decoders;
using TestSupport.EfHelpers;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
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
                var decoded = new DecodedEntityClass(typeof(Author), context);

                //VERIFY
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Author.AuthorId));
                decoded.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeTrue();
                decoded.EntityClassInfo.CanBeCreated.ShouldBeTrue();
                decoded.EntityClassInfo.PropertiesWithPublicSetter.Select(x => x.Name)
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
                var decoded = new DecodedEntityClass(typeof(Review), context);

                //VERIFY
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Review.ReviewId));
                decoded.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.EntityClassInfo.CanBeCreated.ShouldBeFalse();
                decoded.EntityClassInfo.PublicCtors.Length.ShouldEqual(0);
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
                var decoded = new DecodedEntityClass(typeof(Book), context);

                //VERIFY
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Book.BookId));
                decoded.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.EntityClassInfo.CanBeCreated.ShouldBeTrue();
                decoded.EntityClassInfo.PublicCtors.Length.ShouldEqual(1);
                decoded.EntityClassInfo.PublicStaticFactoryMethods.Length.ShouldEqual(0);
                decoded.EntityClassInfo.PublicSetterMethods.Length.ShouldEqual(5);
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
                var decoded = new DecodedEntityClass(typeof(Order), context);

                //VERIFY
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Order.OrderId));
                decoded.EntityClassInfo.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.EntityClassInfo.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.EntityClassInfo.CanBeCreated.ShouldBeTrue();
                decoded.EntityClassInfo.PublicCtors.Length.ShouldEqual(0);
                decoded.EntityClassInfo.PublicStaticFactoryMethods.Length.ShouldEqual(1);
                decoded.EntityClassInfo.PublicSetterMethods.Length.ShouldEqual(1);
            }
        }

    }
}