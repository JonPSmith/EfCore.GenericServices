// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericServices;

namespace Tests.EfClasses
{
    public class DddStaticCreateEntity
    {
        private DddStaticCreateEntity() { }

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