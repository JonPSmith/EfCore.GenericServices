// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using GenericServices.Internal.Decoders;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestBestMethodCtorMatch
    {
        [Theory]
        [InlineData(1, "MyGoodMethod", "")]
        [InlineData(0.9, "MyAverageMethod", "MyGoodMethod")]
        public void TestGetMatchMethods(double expectedScore, string expectedMethod, string excludeThisMethod)
        {
            //SETUP 
            var props = typeof(Dto).GetProperties().ToImmutableList();
            var methods = typeof(InstanceMethods)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => x.Name != excludeThisMethod).ToArray();

            //ATTEMPT
            var bestMethod = BestMethodCtorMatch.FindMatch(props, methods);

            //VERIFY
            bestMethod.ShouldNotBeNull();
            Math.Abs(bestMethod.Score - expectedScore).ShouldBeInRange(0, 0.05);
            bestMethod.Method.Name.ShouldEqual(expectedMethod);
        }

        [Fact]
        public void TestBadMethod()
        {
            //SETUP 
            var props = typeof(Dto).GetProperties().ToImmutableList();
            var method = typeof(InstanceMethods)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Last();

            //ATTEMPT
            var bestMethod = BestMethodCtorMatch.FindMatch(props, new MethodInfo[] {method});

            //VERIFY
            bestMethod.ShouldNotBeNull();
            bestMethod.Score.ShouldEqual(0.5);
            bestMethod.Method.Name.ShouldEqual(nameof(InstanceMethods.MyBadMethod));
        }

        [Fact]
        public void TestToString()
        {
            //SETUP 
            var props = typeof(Dto).GetProperties().ToImmutableList();
            var method = typeof(InstanceMethods)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Last();

            //ATTEMPT
            var bestMethod = BestMethodCtorMatch.FindMatch(props, new MethodInfo[] { method });

            //VERIFY
            bestMethod.ShouldNotBeNull();
            bestMethod.ToString().ShouldEqual("Closest match at 50%: MyBadMethod(Int32 MyInt, String MyString)");
        }

        [Theory]
        [InlineData(1, 3, "")]
        [InlineData(0.85, 2, "MyInt")]
        public void TestGetMatchCtors(double expectedScore, int numParamExpected, string excludeThisProperty)
        {
            //SETUP 
            var props = typeof(Dto).GetProperties().Where(x => x.Name != excludeThisProperty).ToImmutableList();
            var ctors = typeof(Ctors).GetConstructors();

            //ATTEMPT
            var bestMethod = BestMethodCtorMatch.FindMatch(props, ctors);

            //VERIFY
            bestMethod.ShouldNotBeNull();
            Math.Abs(bestMethod.Score - expectedScore).ShouldBeInRange(0, 0.05);
            bestMethod.Constructor.GetParameters().Length.ShouldEqual(numParamExpected);

        }

        private class Dto
        {
            public int MyInt { get; set; }
            public string MyString { get; set; }
            public IEnumerable<int> MyList { get; set; }
        }

        private class InstanceMethods
        {
            public void MyGoodMethod(IEnumerable<int> myList, int myInt, string myString)
            {
            }

            public void MyAverageMethod(string myString, int myInt, ICollection<int> myList)
            {
            }

            public void MyBadMethod(bool myBool, string myString)
            {
            }
        }

        //-----------------------------------------------------------
        //Ctors


        private class Ctors
        {
            public Ctors(IEnumerable<int> myList, int myInt, string myString)
            {
            }

            public Ctors(string myString, ICollection<int> myList)
            {
            }

            public Ctors(bool myBool, string myString)
            {
            }

            private Ctors()
            {
            }

            private Ctors(int myInt)
            {
            }
        }
    }
}