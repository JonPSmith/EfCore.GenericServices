// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore.Design;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestParametersMatch
    {
        public void ParemeterlessMethod() { }
        public void SetMyInt(int myInt) { }

        public void SetMyString(string myString) { }

        public void NotMatch(string myString, int notMyInt) { }

        public int MyInt { get; set; }
        public string MyString { get; set; }

        private static MethodInfo _paremeterlessMethod = typeof(TestParametersMatch).GetMethod(nameof(ParemeterlessMethod));
        private static MethodInfo _setMyInt = typeof(TestParametersMatch).GetMethod(nameof(SetMyInt));
        private static MethodInfo _setMyString = typeof(TestParametersMatch).GetMethod(nameof(SetMyString));
        private static PropertyInfo _myIntProp = typeof(TestParametersMatch).GetProperty(nameof(MyInt));
        private static PropertyInfo _myStringProp = typeof(TestParametersMatch).GetProperty(nameof(MyString));


        [Theory]
        [InlineData(nameof(MyInt), nameof(SetMyInt), 1.0)]
        [InlineData(nameof(MyString), nameof(SetMyInt), 0.0)]
        [InlineData(nameof(MyInt), nameof(ParemeterlessMethod), 1.0)]
        public void TestMatchGood(string propName, string methodName, double expectedScore)
        {
            //SETUP
            var prop = typeof(TestParametersMatch).GetProperty(propName);
            var method = typeof(TestParametersMatch).GetMethod(methodName);

            //ATTEMPT
            var match = new ParametersMatch(method.GetParameters(), new List<PropertyInfo> {prop}, DefaultNameMatcher.MatchCamelAndPascalName);

            //VERIFY
            match.Score.ShouldEqual(expectedScore);
        }

        [Fact]
        public void TestPartialMatch()
        {
            //SETUP
            var props = new List<PropertyInfo> {_myIntProp, _myStringProp};
            var method = typeof(TestParametersMatch).GetMethod(nameof(NotMatch));

            //ATTEMPT
            var match = new ParametersMatch(method.GetParameters(), props, DefaultNameMatcher.MatchCamelAndPascalName);

            //VERIFY
            match.Score.ShouldEqual(0.65);
        }
    }
}