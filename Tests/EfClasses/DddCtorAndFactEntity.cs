// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericLibsBase;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tests.EfClasses
{
    public class DddCtorAndFactEntity
    {
        private DddCtorAndFactEntity() { }

        public DddCtorAndFactEntity(int id, int myInt, string myString)
        {
            Id = id;
            MyInt = myInt;
            MyString = myString;
        }

        public static IStatusGeneric<DddCtorAndFactEntity> CreateFactory(int id, int myInt, string myString)
        {
            var status = new StatusGenericHandler<DddCtorAndFactEntity>();
            var result = new DddCtorAndFactEntity
            {
                Id = id,
                MyInt = myInt,
                MyString = myString,
            };
            if (myString == null)
                status.AddError("The string should not be null.");

            return status.SetResult(result); //This will return null if there are errors
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