// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Xunit;

namespace Tests.UnitTests.DataLayer
{
    public class TestDddBookLoad
    {
        [Fact]
        public void TestLoadBooksOk()
        {
            //SETUP
            //var testsDir = TestData.GetCallingAssemblyTopLevelDir();
            //var dataDir = Path.GetFullPath($"{testsDir}{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}"+
            //    $"{nameof(RazorPageApp)}{Path.DirectorySeparatorChar}wwwroot{Path.DirectorySeparatorChar}{SetupHelpers.SeedFileSubDirectory}");

            ////ATTEMPT
            //var books = BookJsonLoader.LoadBooks(dataDir,
            //        SetupHelpers.SeedDataSearchName).ToList();

            ////VERIFY
            //books.Count.ShouldEqual(53);
            //books.All(x => x.ActualPrice != 0).ShouldBeTrue();
        }
    }

}