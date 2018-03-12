// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController.Services
{
    public class ChangePubDateService : IChangePubDateService
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
                throw new InvalidOperationException($"Could not find the book with Id of {id}.");
            return dto;
        }

        public Book UpdateBook(ChangePubDateDto dto)   
        {
            var book = _context.Find<Book>(dto.BookId);
            book.UpdatePublishedOn(dto.PublishedOn);        
            _context.SaveChanges();                    
            return book;                               
        }
    }
}