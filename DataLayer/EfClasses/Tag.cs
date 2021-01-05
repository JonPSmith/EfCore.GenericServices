// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DataLayer.EfClasses
{
    public class Tag
    {
        private HashSet<Book> _books;

        public Tag(string tagId)
        {
            TagId = tagId;
        }

        [Key]
        [Required]
        [MaxLength(40)]
        public string TagId { get; set; }

        public IEnumerable<Book> Books => _books.ToList();
    }
}