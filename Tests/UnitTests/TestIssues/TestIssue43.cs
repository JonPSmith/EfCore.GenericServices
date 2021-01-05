// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericServices.Setup;
using Tests.Dtos;
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

                //ATTEMPT
                var ex = Assert.Throws<InvalidOperationException>(() => context.SetupSingleDtoAndEntities<DtoLinkedToOwnedType>());

                //VERIFY
                ex.Message.ShouldEqual("DtoLinkedToOwnedType: You cannot use ILinkToEntity<T> with an EF Core Owned Type.");
            }
        }
    }
}