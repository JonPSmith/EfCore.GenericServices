// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using AutoMapper;
using GenericServices;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.TestIssues
{
    public class TestIssue47
    {
        [Fact]
        public void TestDtoWithNoPropertiesForLinkedEntity()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<TestDbContext>();
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new ParentOneToOne {OneToOne = new Child {MyString = "Test"}});
                context.SaveChanges();

                var utData = context.SetupSingleDtoAndEntities<Issue47Dto>();
                var service = new CrudServices(context, utData.ConfigAndMapper);

                //ATTEMPT
                var list = service.ReadManyNoTracked<Issue47Dto>().ToList();

                //VERIFY
                list.Count.ShouldEqual(1);
                list[0].OneToOneMyString.ShouldEqual("Test");
            }
        }

        public class Issue47Dto : ILinkToEntity<ParentOneToOne>
        {
            public int OneToOneChildId { get; set; }
            public string OneToOneMyString { get; set; }
        }

        public class Issue47DtoConfig : PerDtoConfig<Issue47Dto, ParentOneToOne>
        {
        public override Action<IMappingExpression<ParentOneToOne, Issue47Dto>> AlterReadMapping
        {
            get
            {
                return cfg => cfg.ForMember(d => d.OneToOneMyString, 
                    opt => opt.MapFrom(s => s.OneToOne.MyString));
            }
        }
    }
}
}