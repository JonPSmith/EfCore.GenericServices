// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using GenericServices.Internal;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestExtractDisplayHelpers
    {
        private Test _test = new Test();


        [Fact]
        public void TestGetDisplayName()
        {
            //SETUP 
            //ATTEMPT

            //VERIFY
            _test.GetNameForProperty(x => x.I).ShouldEqual("Display Name");
            _test.GetNameForProperty(x => x.J).ShouldEqual("OneName");
            _test.GetNameForProperty(x => x.PascalName).ShouldEqual("Pascal Name");
            _test.GetNameForProperty(x => x.Longwordnotcamel).ShouldEqual("Longwordnotcamel");
        }

        [Fact]
        public void TestClassName()
        {
            //SETUP 
            //ATTEMPT

            //VERIFY
            ExtractDisplayHelpers.GetNameForClass<Test>().ShouldEqual("ClassName");
            ExtractDisplayHelpers.GetNameForClass<TestWithoutDisplayName>().ShouldEqual("Test Without Display Name");
        }

        [Display(Name="ClassName")]
        private class Test
        {
            [Display(Name = "Display Name")]
            public int I { get; set; }

            [Display(Name = "OneName")]
            public int J { get; set; }

            public int PascalName { get; set; }
            public int Longwordnotcamel { get; set; }
        }

        private class TestWithoutDisplayName
        {}

        //[Fact]
        //public void TestGetShortName()
        //{
        //    //SETUP 
        //    //ATTEMPT

        //    //VERIFY
        //    _test.GetShortName(x => x.I).ShouldEqual("Short Name");
        //    _test.GetShortName(x => x.J).ShouldEqual("J");
        //    _test.GetShortName(x => x.PascalName).ShouldEqual("Short Name");
        //    _test.GetShortName(x => x.L).ShouldEqual("L");
        //}
    }
}