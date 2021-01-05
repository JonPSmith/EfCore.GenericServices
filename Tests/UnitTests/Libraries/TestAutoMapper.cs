// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.EfClasses;
using Tests.Configs;
using Tests.Dtos;
using Tests.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.Libraries
{
    public class TestAutoMapper
    {
        [Fact]
        public void TestDirectMappingBookTitle()
        {
            //SETUP
            var wrappedMapper = AutoMapperHelpers.CreateWrapperMapper<Book, BookTitle>();

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().First();
            var data = wrappedMapper.MapperSaveConfig.CreateMapper().Map<BookTitle>(input);

            //VERIFY
            data.Title.ShouldEqual("Refactoring");
        }

        [Fact]
        public void TestProjectionMappingBookTitle()
        {
            //SETUP
            var wrappedMapper = AutoMapperHelpers.CreateWrapperMapper<BookTitle,Book>();

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().AsQueryable();
            var list = input.ProjectTo<BookTitle>(wrappedMapper.MapperReadConfig).ToList();

            //VERIFY
            list.First().Title.ShouldEqual("Refactoring");
        }

        [Fact]
        public void TestDirectMappingBookTitleAndCount()
        {
            //SETUP
            var genSerConfig = new BookTitleAndCountConfig();
            var mapperConfig = AutoMapperHelpers.CreateReadConfig<Book, BookTitleAndCount>(genSerConfig.AlterReadMapping);

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().Last();
            var data = mapperConfig.CreateMapper().Map<BookTitleAndCount>(input);

            //VERIFY
            data.Title.ShouldEqual("Quantum Networking");
            data.ReviewsCount.ShouldEqual(2);
        }

        [Fact]
        public void TestProjectionMappingBookTitleAndCount()
        {
            //SETUP
            var genSerConfig = new BookTitleAndCountConfig();
            var mapperConfig = AutoMapperHelpers.CreateReadConfig<Book, BookTitleAndCount>(genSerConfig.AlterReadMapping);

            //ATTEMPT
            var input = DddEfTestData.CreateFourBooks().AsQueryable();
            var list = input.ProjectTo<BookTitleAndCount>(mapperConfig).ToList();

            //VERIFY
            list.Last().Title.ShouldEqual("Quantum Networking");
            list.Last().ReviewsCount.ShouldEqual(2);
        }

        [Fact]
        public void TestProjectionMappingBookTitleBadType()
        {
            //SETUP
            var wrappedMapper = AutoMapperHelpers.CreateWrapperMapper<Book, BookTitleBadType>();

            //ATTEMPT
            wrappedMapper.MapperSaveConfig.AssertConfigurationIsValid();

            //VERIFY
            //Doesn't error on name fit but different 
        }

        [Fact]
        public void TestDirectMappingToBookNotSetPrivateSetter()
        {
            //SETUP
            var wrappedMapper = AutoMapperHelpers.CreateWrapperMapper<BookTitle, Book>();
            var entity = DddEfTestData.CreateFourBooks().First();

            //ATTEMPT
            var dto = new BookTitle {Title = "New Title"};
            var data = wrappedMapper.MapperSaveConfig.CreateMapper().Map(dto, entity);

            //VERIFY
            data.Title.ShouldEqual("Refactoring");
        }

        private bool Filter(MemberInfo member)
        {
            var readOnlyAttr = member.GetCustomAttribute<ReadOnlyAttribute>();
            var isReadOnly = readOnlyAttr?.IsReadOnly ?? false;
            return isReadOnly;
        }

        [Fact]
        public void TestIgnoreReadOnlyProperties()
        {
            //SETUP
            var entity = new Author{AuthorId = 1, Name = "Start Name", Email = "me@nospam.com"};
            var config = new MapperConfiguration(cfg =>
            {
                //see https://github.com/AutoMapper/AutoMapper/issues/2571#issuecomment-374159340
                cfg.ForAllPropertyMaps(pm => Filter(pm.SourceMember), (pm, opt) => opt.Ignore());
                cfg.CreateMap<WriteAuthorReadOnlyDto, Author>();
            });

            //ATTEMPT
            var dto = new WriteAuthorReadOnlyDto { AuthorId = 123, Name = "New Name", Email = "youhavechanged@gmail.com"};
            var mapper = config.CreateMapper();
            var data = mapper.Map(dto, entity);

            //VERIFY
            data.Name.ShouldEqual("New Name");       //changed
            data.AuthorId.ShouldEqual(123);          //changed
            data.Email.ShouldEqual("me@nospam.com"); //not changed - ReadOnly
        }

        [Fact]
        public void TestUsingProfileBasic()
        {
            //SETUP
            var entity = new Author { AuthorId = 1, Name = "Start Name", Email = "me@nospam.com" };
            var profile = new UnitTestProfile(false);
            profile.AddWriteMap<WriteAuthorReadOnlyDto, Author>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            //ATTEMPT
            var dto = new WriteAuthorReadOnlyDto { AuthorId = 123, Name = "New Name", Email = "youhavechanged@gmail.com" };
            var mapper = config.CreateMapper();
            var data = mapper.Map(dto, entity);

            //VERIFY
            data.Name.ShouldEqual("New Name");       
            data.AuthorId.ShouldEqual(123);            
            data.Email.ShouldEqual("youhavechanged@gmail.com");    
        }

        [Fact]
        public void TestUsingProfileWithIgnore()
        {
            //SETUP
            var entity = new Author { AuthorId = 1, Name = "Start Name", Email = "me@nospam.com" };
            var profile = new UnitTestProfile(true);
            profile.AddWriteMap<WriteAuthorReadOnlyDto, Author>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(profile));

            //ATTEMPT
            var dto = new WriteAuthorReadOnlyDto { AuthorId = 123, Name = "New Name", Email = "youhavechanged@gmail.com" };
            var mapper = config.CreateMapper();
            var data = mapper.Map(dto, entity);

            //VERIFY
            data.Name.ShouldEqual("New Name");       //changed
            data.AuthorId.ShouldEqual(123);          //changed
            data.Email.ShouldEqual("me@nospam.com"); //not changed - ReadOnly
        }

        [Fact]
        public void TestUsingProfileWithIgnoreDoesntAffectOtherProfiles()
        {
            //SETUP
            var profile1 = new UnitTestProfile(true);
            profile1.AddWriteMap<WriteAuthorReadOnlyDto, Author>();
            var config1 = new MapperConfiguration(cfg => cfg.AddProfile(profile1));
            var profile2 = new UnitTestProfile(false);
            profile2.AddWriteMap<WriteAuthorReadOnlyDto, Author>();
            var config2 = new MapperConfiguration(cfg => cfg.AddProfile(profile2));

            //ATTEMPT
            var dto = new WriteAuthorReadOnlyDto { AuthorId = 123, Name = "New Name", Email = "youhavechanged@gmail.com" };
            var entity1 = new Author { AuthorId = 1, Name = "Start Name", Email = "me@nospam.com" };
            config1.CreateMapper().Map(dto, entity1);
            var entity2 = new Author { AuthorId = 1, Name = "Start Name", Email = "me@nospam.com" };
            config2.CreateMapper().Map(dto, entity2);

            //VERIFY
            entity1.Email.ShouldEqual("me@nospam.com"); //not changed - ReadOnly
            entity2.Email.ShouldEqual("youhavechanged@gmail.com"); //changes
        }
    }
}