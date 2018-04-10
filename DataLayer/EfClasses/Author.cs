// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.EfClasses
{
    //I have styled the Author entity class as a standard-styled entity, 
    //i.e. it can be created/updated via its property setters. 
    //Technically it has to have a public, parameterless constructor and all properties should  have public setters
    public class Author
    {
        public const int NameLength = 100;
        public const int EmailLength = 100;

        public Author() { }

        public int AuthorId { get;  set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(NameLength)]
        public string Name { get;  set; }

        [MaxLength(EmailLength)]
        public string Email { get; set; }

        //------------------------------
        //Relationships

        public ICollection<BookAuthor> BooksLink { get; set; }
    }

}