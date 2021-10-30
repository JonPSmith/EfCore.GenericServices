// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Tests.EfClasses;
using Tests.EfCode;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

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
            var options = SqliteInMemory.CreateOptionsWithLogTo<TestDbContext>(log => _output.WriteLine(log.ToString()));
            using (var context = new TestDbContext(options))
            {
                context.Database.EnsureCreated();
                context.ExecuteScriptFileInTransaction(TestData.GetFilePath("ReplaceTableWithView.sql"));

                context.Add(new Parent
                    {Children = new List<Child> {new Child {MyString = "Hello"}, new Child {MyString = "Goodbye"}}});
                context.SaveChanges();

                context.ChangeTracker.Clear();

                //ATTEMPT
                var children = context.Children.ToList();

                //VERIFY
                children.Count.ShouldEqual(2);
            }
        }
    }

}