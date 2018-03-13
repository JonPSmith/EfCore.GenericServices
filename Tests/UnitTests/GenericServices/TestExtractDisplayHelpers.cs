using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Xunit;
using GenericServices.Internal;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServices
{
    public class TestExtractDisplayHelpers
    {
        private Test _test;

        [DisplayName("ClassName")]
        private class Test
        {
            [Display(Name = "Display Name", ShortName = "Short Name")]
            public int I { get; set; }
            [Display(Name = "OneName")]
            public int J { get; set; }
            [Display(ShortName = "Short Name")]
            public int CamelName { get; set; }
            public int Longwordnotcamel { get; set; }
        }

        private class TestWithoutDisplayName
        {

        }


        [Fact]
        public void TestGetDisplayName()
        {
            //SETUP 
            //ATTEMPT

            //VERIFY
            _test.GetNameForProperty(x => x.I).ShouldEqual("Display Name");
            _test.GetNameForProperty(x => x.J).ShouldEqual("OneName");
            _test.GetNameForProperty(x => x.CamelName).ShouldEqual("Camel Name");
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

        //[Fact]
        //public void TestGetShortName()
        //{
        //    //SETUP 
        //    //ATTEMPT

        //    //VERIFY
        //    _test.GetShortName(x => x.I).ShouldEqual("Short Name");
        //    _test.GetShortName(x => x.J).ShouldEqual("J");
        //    _test.GetShortName(x => x.CamelName).ShouldEqual("Short Name");
        //    _test.GetShortName(x => x.L).ShouldEqual("L");
        //}
    }
}