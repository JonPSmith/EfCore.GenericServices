using System;
using System.Collections.Generic;
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
            var action = method.CallMethodReturnVoid<Dto, Target1>(prop);
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
            var action = method.CallMethodReturnStatus<Dto, Target1>(prop);
            var status = action.Invoke(dto, target);

            //VERIFY
            target.MyString.ShouldEqual("Hello");
            status.Message.ShouldEqual("OK");
        }

    }
}