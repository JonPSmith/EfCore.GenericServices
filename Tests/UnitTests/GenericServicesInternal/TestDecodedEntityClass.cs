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
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeTrue();
                decoded.CanBeCreated.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(1);
                decoded.PublicStaticFactoryMethods.Length.ShouldEqual(0);
                decoded.PublicSetterMethods.Length.ShouldEqual(0);
                decoded.PropertiesWithPublicSetter.Select(x => x.Name)
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
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.CanBeCreated.ShouldBeFalse();
                decoded.PublicCtors.Length.ShouldEqual(0);
                decoded.PublicStaticFactoryMethods.Length.ShouldEqual(0);
                decoded.PublicSetterMethods.Length.ShouldEqual(0);
                decoded.PropertiesWithPublicSetter.Length.ShouldEqual(0);
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
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.CanBeCreated.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(1);
                decoded.PublicStaticFactoryMethods.Length.ShouldEqual(0);
                decoded.PublicSetterMethods.Length.ShouldEqual(5);
                decoded.PropertiesWithPublicSetter.Length.ShouldEqual(0);
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
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.CanBeCreated.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(0);
                decoded.PublicStaticFactoryMethods.Length.ShouldEqual(1);
                decoded.PublicSetterMethods.Length.ShouldEqual(1);
                decoded.PropertiesWithPublicSetter.Length.ShouldEqual(0);
            }
        }

    }
}