
using System;

namespace Tests.EfClasses
{
    public class ContactAddress 
    {
        public int ContactAddressId { get; private set; }

        public Address Address { get; private set; }

        private ContactAddress() { }

        public ContactAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }
}