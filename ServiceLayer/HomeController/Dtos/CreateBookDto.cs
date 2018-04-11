// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DataLayer.EfClasses;
using GenericServices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ServiceLayer.HomeController.Dtos
{
    public class CreateBookDto : ILinkToEntity<Book>
    {
        //This will be populated with the primary key of the created book
        public int BookId { get; set; }

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

        public CreateBookDto()
        {
            PublishedOn = DateTime.Today;
        }

        //---------------------------------------------------------
        //Now the data for the front end

        public struct IdText
        {
            public IdText(int id, string text)
            {
                Id = id;
                Text = text;
            }

            [JsonProperty(PropertyName = "id")]
            public int Id { get; }
            [JsonProperty(PropertyName = "text")]
            public string Text { get; }
        }

        public List<IdText> AuthorList { get; private set; }

        public void BeforeDisplay(DbContext context)
        {
            AuthorList = context.Set<Author>().Select(x => new IdText(x.AuthorId, x.Name))
                .OrderBy(x => x.Text).ToList();
        }

        public List<int> BookAuthorIds { get; set; } = new List<int>();

        public void BeforeSave(DbContext context)
        {
            Authors = BookAuthorIds.Select(x => context.Find<Author>(x)).Where(x => x != null).ToList();
        }
    }
}