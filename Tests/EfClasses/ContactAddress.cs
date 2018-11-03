
using System;

namespace Tests.EfClasses
{
    public class ContactAddress 
    {
        public int ContactAddressId { get; private set; }

        public string Name { get; set; }

        public AddressNotOwned Address { get; set; }
    }
}