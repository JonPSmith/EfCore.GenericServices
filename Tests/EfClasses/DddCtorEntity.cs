// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericLibsBase;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tests.EfClasses
{
    public class DddCtorEntity
    {
        private DddCtorEntity() { }

        public DddCtorEntity(int id, int myInt, string myString)
        {
            Id = id;
            MyInt = myInt;
            MyString = myString;
        }

        public int Id { get; private set; }
        public int MyInt { get; private set; }
        public string MyString { get; private set; }


        public void SetInt(int myInt)
        {
            MyInt = myInt;
        }

        public IStatusGeneric SetString(string myString)
        {
            var status = new StatusGenericHandler();
            if (myString == null)
            {
                return status.AddError("The string should not be null.");
            }

            MyString = myString;
            return status;
        }
    }
}