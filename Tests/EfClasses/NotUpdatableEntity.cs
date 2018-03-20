// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Internal;

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