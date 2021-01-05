// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using GenericServices.Configuration;
using GenericServices.Internal.LinqBuilders;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesInternal
{
    public class TestBuildCallAsync
    {
        [Fact]
        public async Task TestBuildCallTaskMethodNoReturn()
        {
            //SETUP 
            var prop = new PropertyMatch(true, PropertyMatch.TypeMatchLevels.Match, typeof(Dto).GetProperty(nameof(Dto.MyInt)));
            var method = typeof(Target1).GetMethod(nameof(Target1.MyTask));
            var dto = new Dto {MyInt = 123};
            var target = new Target1();

            //ATTEMPT
            var action = BuildCallAsync.TaskCallMethodReturnVoid(method, typeof(Dto), typeof(Target1), new List<PropertyMatch>{prop});
            await action.Invoke(dto, target);

            //VERIFY
            target.MyInt.ShouldEqual(123);
        }

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

            public async Task MyTask(int myInt)
            {
                MyInt = myInt;
                await Task.Delay(10);
            }
        }
    }
}