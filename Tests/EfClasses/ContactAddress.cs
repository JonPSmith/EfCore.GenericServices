// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public class ContactAddress 
    {
        public int ContactAddressId { get; private set; }

        public string Name { get; set; }

        public AddressNotOwned Address { get; set; }
    }
}