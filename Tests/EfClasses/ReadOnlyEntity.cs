// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public class ReadOnlyEntity
    {
        private ReadOnlyEntity() { }
        public int Id { get; private set; }
        public int MyInt { get; private set; }
        public string MyString { get; private set; }
    }
}