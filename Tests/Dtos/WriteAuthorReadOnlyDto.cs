// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;

namespace Tests.Dtos
{
    public class WriteAuthorReadOnlyDto
    {
        [ReadOnly(true)]
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(Author.NameLength)]
        public string Name { get; set; }

        [MaxLength(Author.EmailLength)]
        public string Email { get; set; }
    }
}