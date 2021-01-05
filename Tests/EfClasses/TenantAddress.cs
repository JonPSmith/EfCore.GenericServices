// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Tests.EfClasses
{
    public class TenantAddress
    {
        private TenantAddress() { }

        public TenantAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public int TenantAddressId { get; private set; }

        public Address Address { get; private set; }
    }
}