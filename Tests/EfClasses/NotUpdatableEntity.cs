// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public class NotUpdatableEntity
    {
        private NotUpdatableEntity() { }

        public NotUpdatableEntity(int id, int myInt, string myString)
        {
            Id = id;
            MyInt = myInt;
            MyString = myString;
        }

        public int Id { get; private set; }
        public int MyInt { get; private set; }
        public string MyString { get; private set; }
    }
}