// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue43
    {
        [Fact]
        public void TestThrowExceptionIfILinkToEntityIsOwnedType()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                var utData = context.SetupSingleDtoAndEntities<DtoLinkedToOwnedType>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new DtoLinkedToOwnedType();
                var ex = Assert.Throws<InvalidOperationException>(() => service.UpdateAndSave(dto));

                //VERIFY
                ex.Message.ShouldEqual("The class Address of style OwnedType cannot be used in Update.");
            }
        }
    }
}