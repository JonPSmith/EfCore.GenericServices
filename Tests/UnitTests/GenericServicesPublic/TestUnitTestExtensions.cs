// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Extensions;
using Microsoft.EntityFrameworkCore;
using Tests.Dtos;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestUnitTestExtensions
    {
        private readonly ITestOutputHelper _output;

        public TestUnitTestExtensions(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestCheckSetupEntitiesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var status = context.CheckSetupEntities();

                //VERIFY
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors));
                _output.WriteLine(status.Message);
            }
        }

        [Fact]
        public void TestCheckDtosInAssemblyWithOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var status = context.CheckDtosInAssemblyWith<BookTitle>();

                //VERIFY
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors));
                _output.WriteLine(status.Message);
            }
        }

    }

}