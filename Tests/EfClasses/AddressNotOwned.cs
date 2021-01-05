// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Tests.EfClasses
{

    public class AddressNotOwned
    {
        public int Id { get; set; }

        [Required]
        public string Address1 { get; set; }

        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateOrProvice { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
}