// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace Tests.Dtos
{
    public class CreatorOfBooksDto : ILinkToEntity<Book>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        [Range(0, 1000)]
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public ICollection<Author> Authors { get; set; }    
    }
}