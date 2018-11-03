// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.Dtos;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.GenericServicesPublic
{
    public class TestNestedInDtosIssue13
    {

        [Fact]
        public void TestInDtosNestedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();

                var utData = context.SetupSingleDtoAndEntities<InContactAddressDto>();
                utData.AddSingleDto<InAddressDto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var dto = new InContactAddressDto
                {
                    Name = "test",
                    Addess = new InAddressDto
                    {
                        Address1 = "some street"
                    }
                };
                service.CreateAndSave(dto);

                //VERIFY
                service.IsValid.ShouldBeTrue(service.GetAllErrors());
                var contact = context.ContactAddresses.SingleOrDefault();
                contact.Name.ShouldEqual("test");
                contact.Address.Address1.ShouldEqual("some street");
            }
        }

        

    }
}