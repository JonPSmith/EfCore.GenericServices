// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericServices;

namespace ServiceLayer.HomeController.Dtos
{
    public class DeleteBookDto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string AuthorsOrdered { get; set; }
    }
}