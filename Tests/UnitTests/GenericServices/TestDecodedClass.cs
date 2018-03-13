using System;
using GenericLibsBase;
using Xunit;
using GenericServices.Internal.Decoders;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServices
{
    public class TestDecodedClass
    {
        private class NormalClass
        {
            public int Id { get; set; }
            public string MyString { get; set; }

            public DateTime PrivateSetter { get; private set; }

            private void MyMethod() { }
        }

        private class LockedClass
        {
            public int Id { get; private set; }
            public string MyString { get; private set; }

            private LockedClass() { }

            public LockedClass(int id, string myString)
            {
                Id = id;
                MyString = myString ?? throw new ArgumentNullException(nameof(myString));
            }

            public void SetId(int id)
            {
                Id = id;
            }

            public IStatusGeneric SetMyString(string myString)
            {
                MyString = myString;
                var status = new StatusGenericHandler();
                if (myString == null)
                    status.AddError("bad");
                return status;
            }
        }

        internal class LockedClassStaticFactory
        {
            public int Id { get; private set; }
            public string MyString { get; private set; }

            private LockedClassStaticFactory() { }

            private LockedClassStaticFactory(int id, string myString)
            {
                Id = id;
                MyString = myString;
            }

            public static IStatusGeneric<LockedClassStaticFactory> CreateFactory(int id, string myString)
            {
                var status = new StatusGenericHandler<LockedClassStaticFactory>();
                status.Result = new LockedClassStaticFactory(id, myString);
                return status;
            }

            public void SetId(int id)
            {
                Id = id;
            }

            public IStatusGeneric SetMyString(string myString)
            {
                MyString = myString;
                var status = new StatusGenericHandler();
                if (myString == null)
                    status.AddError("bad");
                return status;
            }
        }


        [Fact]
        public void NormalClassDecode()
        {
            //SETUP 
            //ATTEMPT
            var decoded = new DecodedClass(typeof(NormalClass));

            //VERIFY
            decoded.CanBeUpdatedViaProperties.ShouldBeTrue();
            decoded.CanBeUpdatedViaMethods.ShouldBeFalse();
            decoded.PublicCtors.Length.ShouldEqual(1);
            decoded.publicSetterMethods.Length.ShouldEqual(0);
            decoded.publicStaticFactoryMethods.Length.ShouldEqual(0);
            decoded.propertiesWithPublicSetter.Length.ShouldEqual(2);
        }

        [Fact]
        public void LockedClassDecode()
        {
            //SETUP 
            //ATTEMPT
            var decoded = new DecodedClass(typeof(LockedClass));

            //VERIFY
            decoded.CanBeUpdatedViaProperties.ShouldBeFalse();
            decoded.CanBeUpdatedViaMethods.ShouldBeTrue();
            decoded.PublicCtors.Length.ShouldEqual(1);
            decoded.publicSetterMethods.Length.ShouldEqual(2);
            decoded.publicStaticFactoryMethods.Length.ShouldEqual(0);
            decoded.propertiesWithPublicSetter.Length.ShouldEqual(0);
        }


    }
}