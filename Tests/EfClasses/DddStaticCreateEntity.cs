// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using StatusGeneric;

namespace Tests.EfClasses
{
    public class DddStaticCreateEntity
    {
        private DddStaticCreateEntity() { }

        public int Id { get; private set; }
        public int MyInt { get; private set; }
        public string MyString { get; private set; }

        public static IStatusGeneric<DddStaticCreateEntity> Create(int myInt, string myString)
        {
            var status = new StatusGenericHandler<DddStaticCreateEntity>();
            var result = new DddStaticCreateEntity
            {
                MyInt = myInt,
                MyString = myString,
            };
            if (myString == null)
                status.AddError("The string should not be null.");

            return status.SetResult(result); //This will return null if there are errors
        }


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