// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;

namespace ServiceLayer.HomeController.Dtos
{
    public class CreateBookDto
    {
        //I would normally have the Required attribute to catch this at the front end
        //But to show how the static create method catches that error I have commented it out
        //[Required(AllowEmptyStrings = false)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        [Range(0,1000)]
        public decimal OrgPrice { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Author> Authors { get; set; }
    }
}