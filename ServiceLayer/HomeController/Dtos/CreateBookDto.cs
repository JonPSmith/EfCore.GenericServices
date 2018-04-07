// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.HomeController.Dtos
{
    public class CreateBookDto : ILinkToEntity<Book>
    {
        //I would normally have the Required attribute to catch this at the front end
        //But to show how the static create method catches that error I have commented it out
        //[Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        [Range(0,1000)]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<Author> Authors { get; set; }

        //------------------------------------
        //I cheat and have a simple way to enter Authors

        public string[] AuthorNames { get; set; } = new string[3];

        public void SetupAuthorsCollection(DbContext context)
        {
            Authors = AuthorNames.Select(x => FindOrCreateAuthor(context, x)).Where(x => x != null).ToList();
        }

        private Author FindOrCreateAuthor(DbContext context, string authorsName)
        {
            if (string.IsNullOrEmpty(authorsName))
                return null;

            return context.Set<Author>().FirstOrDefault(x => x.Name == authorsName)
                   ?? new Author {Name = authorsName};

        }
    }
}