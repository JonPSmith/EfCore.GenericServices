// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.HomeController.Dtos;
using StatusGeneric;

namespace ServiceLayer.HomeController.Services
{
    public class ChangePubDateService : StatusGenericHandler, IChangePubDateService
    {
        private readonly EfCoreContext _context;

        public ChangePubDateService(EfCoreContext context)
        {
            _context = context;
        }

        public ChangePubDateDto GetOriginal(int id)    
        {
            var dto = _context.Books
                .Select(p => new ChangePubDateDto      
                {                                      
                    BookId = p.BookId,                 
                    Title = p.Title,                   
                    PublishedOn = p.PublishedOn        
                })                                     
                .SingleOrDefault(k => k.BookId == id); 
            if (dto == null)
                AddError("Sorry, I could not find the book you were looking for.");
            return dto;
        }

        public Book UpdateBook(int id, ChangePubDateDto dto)   
        {
            //Should return error message on not found
            var book = _context.Find<Book>(id);
            if (book == null)
            {
                AddError("Sorry, I could not find the book you were looking for.");
                return null;
            }
            
            book.UpdatePublishedOn(dto.PublishedOn);        
            _context.SaveChanges();                    
            return book;                               
        }
    }
}