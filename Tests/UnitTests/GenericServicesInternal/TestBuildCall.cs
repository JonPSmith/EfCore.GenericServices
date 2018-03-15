using System;
using System.Collections.Generic;
using System.Linq;
using GenericLibsBase;
using GenericServices.Internal.LinqBuilders;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestBuildCall
    {

        private class Dto
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
            public IEnumerable<int> MyList { get; set; }
        }

        private class Target1
        {
            public Target1() { }

            public Target1(int myInt, string myString)
            {
                MyInt = myInt;
                MyString = myString;
            }

            public int MyInt { get; set; }
            public string MyString { get; set; }

            public void SetMyInt(int myInt)
            {
                MyInt = myInt;
            }

            public IStatusGeneric SetMyString(string myString)
            {
                MyString = myString;
                return new StatusGenericHandler {Message = "OK"};
            }

            public static IStatusGeneric<Target1> CreateFactory(int myInt, string myString)
            {
                var status =
                    new StatusGenericHandler<Target1>
                    {
                        Result = new Target1( myInt, myString),
                        Message = "Static"
                    };
                return status;
            }

        }

        [Fact]
        public void TestBuildCallMethodNoReturn()
        {
            //SETUP 
            var prop = typeof(Dto).GetProperty(nameof(Dto.MyInt));
            var method = typeof(Target1).GetMethod(nameof(Target1.SetMyInt));
            var dto = new Dto {MyInt = 123};
            var target = new Target1();

            //ATTEMPT
            var action = method.CallMethodReturnVoid(typeof(Dto), typeof(Target1), prop);
            action.Invoke(dto, target);

            //VERIFY
            target.MyInt.ShouldEqual(123);
        }

        [Fact]
        public void TestBuildCallMethodNoReturnAgain()
        {
            //SETUP 
            var prop = typeof(Dto).GetProperty(nameof(Dto.MyInt));
            var method = typeof(Target1).GetMethod(nameof(Target1.SetMyInt));
            var dto = new Dto { MyInt = 123 };
            var target = new Target1();

            //ATTEMPT
            var action = method.CallMethodReturnVoid(typeof(Dto), typeof(Target1), prop);
            action.Invoke(dto, target);

            //VERIFY
            target.MyInt.ShouldEqual(123);
        }

        [Fact]
        public void TestBuildCallMethodWithReturn()
        {
            //SETUP 
            var prop = typeof(Dto).GetProperty(nameof(Dto.MyString));
            var method = typeof(Target1).GetMethod(nameof(Target1.SetMyString));
            var dto = new Dto { MyString = "Hello" };
            var target = new Target1();

            //ATTEMPT
            var action = method.CallMethodReturnStatus(typeof(Dto), typeof(Target1), prop);
            var status = action.Invoke(dto, target);

            //VERIFY
            target.MyString.ShouldEqual("Hello");
            ((string)status.Message).ShouldEqual("OK");
        }

        [Fact]
        public void TestBuildCallStaticFactory()
        {
            //SETUP 
            var prop1 = typeof(Dto).GetProperty(nameof(Dto.MyInt));
            var prop2 = typeof(Dto).GetProperty(nameof(Dto.MyString));
            var method = typeof(Target1).GetMethod(nameof(Target1.CreateFactory));
            var dto = new Dto { MyInt = 123, MyString = "Hello" };

            //ATTEMPT
            var action = method.CallStaticFactory(typeof(Dto), prop1, prop2);
            var status = action.Invoke(dto);

            //VERIFY
            ((string)status.Message).ShouldEqual("Static");
            ((int)status.Result.MyInt).ShouldEqual(123);
            ((string)status.Result.MyString).ShouldEqual("Hello");
        }

        [Fact]
        public void TestBuildCallCtor()
        {
            //SETUP 
            var prop1 = typeof(Dto).GetProperty(nameof(Dto.MyInt));
            var prop2 = typeof(Dto).GetProperty(nameof(Dto.MyString));
            var ctor = typeof(Target1).GetConstructors().Single(x => x.GetParameters().Length == 2);
            var dto = new Dto { MyInt = 123, MyString = "Hello" };

            //ATTEMPT
            var action = ctor.CallConstructor(typeof(Dto), prop1, prop2);
            var newInstance = action.Invoke(dto);

            //VERIFY
            ((int)newInstance.MyInt).ShouldEqual(123);
            ((string)newInstance.MyString).ShouldEqual("Hello");
        }

    }
}