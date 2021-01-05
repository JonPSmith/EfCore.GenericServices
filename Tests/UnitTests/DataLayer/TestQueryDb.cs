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
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;
using Xunit.Sdk;

namespace Tests.UnitTests.DataLayer
{
    public class TestQueryDb
    {
        private readonly ITestOutputHelper _output;

        public TestQueryDb(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestDbQueryChildReadOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogging<TestDbContext>(log => _output.WriteLine(log.ToString()));
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));

                context.Add(new Parent
                    {Children = new List<Child> {new Child {MyString = "Hello"}, new Child {MyString = "Goodbye"}}});
                context.SaveChanges();
            }

            using (var context = new TestDbContext(options))
            {
                //ATTEMPT
                var children = context.Children.ToList();

                //VERIFY
                children.Count.ShouldEqual(2);
            }
        }


    }

}