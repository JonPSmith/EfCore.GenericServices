// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Dtos;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Tests.EfClasses;
using Tests.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.DataLayer
{
    public class TestQueryDb
    {


        [Fact]
        public void TestDbQueryChildReadOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<QueryDbContext>();
            using (var context = new QueryDbContext(options))
            {
                context.Database.EnsureCreated();
                context.Add(new Parent
                    {Children = new List<Child> {new Child {MyString = "Hello"}, new Child {MyString = "Goodbye"}}});
                context.SaveChanges();
            }

            using (var context = new QueryDbContext(options))
            {
                //ATTEMPT
                var children = context.Children.ToList();

                //VERIFY
                children.Count.ShouldEqual(2);
            }
        }
  

    }

}