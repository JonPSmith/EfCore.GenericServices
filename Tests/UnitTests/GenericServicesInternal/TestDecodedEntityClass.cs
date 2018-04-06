using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Xunit;
using GenericServices.Internal.Decoders;
using Tests.EfClasses;
using Tests.EfCode;
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
                decoded.EntityStyle.ShouldEqual(EntityStyles.Standard);
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Author.AuthorId));
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeTrue();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeFalse();
                decoded.PublicCtors.Length.ShouldEqual(1);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(0);
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
                decoded.EntityStyle.ShouldEqual(EntityStyles.ReadOnly);
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Review.ReviewId));
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeFalse();
                decoded.PublicCtors.Length.ShouldEqual(0);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(0);
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
                decoded.EntityStyle.ShouldEqual(EntityStyles.Hybrid);
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Book.BookId));
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeUpdatedViaProperties.ShouldBeTrue();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(1);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(0);
                decoded.PublicSetterMethods.Length.ShouldEqual(5);
                decoded.PropertiesWithPublicSetter.Length.ShouldEqual(1);
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
                decoded.EntityStyle.ShouldEqual(EntityStyles.DDDStyled);
                decoded.PrimaryKeyProperties.Single().Name.ShouldEqual(nameof(Order.OrderId));
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(0);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(1);
                decoded.PublicSetterMethods.Length.ShouldEqual(1);
                decoded.PropertiesWithPublicSetter.Length.ShouldEqual(0);
            }
        }

        //---------------------------------------------------
        //TestDbContext

        [Fact]
        public void TestNormalEntityDecoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var decoded = new DecodedEntityClass(typeof(NormalEntity), context);

                //VERIFY
                decoded.EntityStyle.ShouldEqual(EntityStyles.Standard);
                decoded.CanBeCreatedViaAutoMapper.ShouldBeTrue();
                decoded.CanBeUpdatedViaProperties.ShouldBeTrue();
                decoded.HasPublicParameterlessCtor.ShouldBeTrue();
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeFalse();
                decoded.PublicSetterMethods.Length.ShouldEqual(0);
                decoded.PropertiesWithPublicSetter.Length.ShouldEqual(3);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDddCtorEntityDecoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var decoded = new DecodedEntityClass(typeof(DddCtorEntity), context);

                //VERIFY
                decoded.EntityStyle.ShouldEqual(EntityStyles.DDDStyled);
                decoded.CanBeCreatedViaAutoMapper.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.HasPublicParameterlessCtor.ShouldBeFalse();
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(2);
                decoded.PublicSetterMethods.Length.ShouldEqual(2);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDddStaticFactEntityDecoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var decoded = new DecodedEntityClass(typeof(DddStaticCreateEntity), context);

                //VERIFY
                decoded.EntityStyle.ShouldEqual(EntityStyles.DDDStyled);
                decoded.CanBeCreatedViaAutoMapper.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.HasPublicParameterlessCtor.ShouldBeFalse();
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(0);
                decoded.PublicSetterMethods.Length.ShouldEqual(2);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDddCtorAndFactEntityDecoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var decoded = new DecodedEntityClass(typeof(DddCtorAndFactEntity), context);

                //VERIFY
                decoded.EntityStyle.ShouldEqual(EntityStyles.DDDStyled);
                decoded.CanBeCreatedViaAutoMapper.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.HasPublicParameterlessCtor.ShouldBeFalse();
                decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(1);
                decoded.PublicSetterMethods.Length.ShouldEqual(2);
                decoded.PublicStaticCreatorMethods.Length.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestNotUpdatableEntityDecoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var decoded = new DecodedEntityClass(typeof(NotUpdatableEntity), context);

                //VERIFY
                decoded.EntityStyle.ShouldEqual(EntityStyles.NotUpdatable);
                decoded.CanBeCreatedViaAutoMapper.ShouldBeFalse();
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.HasPublicParameterlessCtor.ShouldBeFalse();
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeTrue();
                decoded.PublicCtors.Length.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestReadOnlyEntityDecoded()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var decoded = new DecodedEntityClass(typeof(ReadOnlyEntity), context);

                //VERIFY
                decoded.EntityStyle.ShouldEqual(EntityStyles.ReadOnly);
                decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
                decoded.HasPublicParameterlessCtor.ShouldBeFalse();
                decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
                decoded.CanBeCreatedByCtorOrStaticMethod.ShouldBeFalse();
            }
        }

    }
}