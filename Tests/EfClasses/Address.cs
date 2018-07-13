
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tests.EfClasses
{
    [Owned]
    public class Address
    {
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateOrProvice { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
    }
}