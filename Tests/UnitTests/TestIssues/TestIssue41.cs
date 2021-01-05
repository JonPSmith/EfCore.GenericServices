// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue41
    {
        [Fact]
        public void TestThrowExceptionIfDtoPropertiesAreAllReadOnly()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = context.SetupSingleDtoAndEntities<NormalEntityAllMarkedReadOnlyDto>();

                //ATTEMPT
                var service = new CrudServices(context, utData.ConfigAndMapper); ///

                //VERIFY
                
            }
        }
    }
}