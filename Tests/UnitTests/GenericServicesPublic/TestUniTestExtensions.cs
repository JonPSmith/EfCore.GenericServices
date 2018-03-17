// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices.Extensions;
using Microsoft.EntityFrameworkCore;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestUniTestExtensions
    {
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
                status.HasErrors.ShouldBeFalse(string.Join("\n", status.Errors.ToString()));
            }
        }   

    }

}