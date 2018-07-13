using System;

namespace Tests.EfClasses
{
    public class TenantAddress
    {
        public int TenantAddressId { get; private set; }

        public Address Address { get; private set; }

        private TenantAddress() { }

        public TenantAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }
    }
}