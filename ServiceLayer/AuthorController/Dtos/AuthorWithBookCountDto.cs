// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;
using GenericServices;

namespace ServiceLayer.AuthorController.Dtos
{
    public class AuthorWithBookCountDto : ILinkToEntity<Author>
    {
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(Author.NameLength)]
        public string Name { get; set; }

        public int BooksLinkCount { get; set; }
    }
}