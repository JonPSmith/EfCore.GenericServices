// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericLibsBase;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController.Services
{
    public class AddRemovePromotionService : IAddRemovePromotionService
    {
        private readonly EfCoreContext _context;

        public AddRemovePromotionService(EfCoreContext context)
        {
            _context = context;
        }

        public IStatusGeneric Status { get; private set; } = new StatusGenericHandler();

        public AddRemovePromotionDto GetOriginal(int id)      
        {
            var dto = _context.Books
                .Select(p => new AddRemovePromotionDto
                {
                    BookId = p.BookId,
                    Title = p.Title,
                    OrgPrice = p.OrgPrice,
                    ActualPrice = p.ActualPrice,
                    PromotionalText = p.PromotionalText
                })
                .SingleOrDefault(k => k.BookId == id);
            if (dto == null)
                throw new InvalidOperationException($"Could not find the book with Id of {id}.");
            return dto;
        }

        public Book AddPromotion(AddRemovePromotionDto dto)
        {
            var book = _context.Find<Book>(dto.BookId);
            if (book == null)
                throw new InvalidOperationException($"Could not find the book with Id of {dto.BookId}.");
            Status = book.AddPromotion(dto.ActualPrice, dto.PromotionalText);
            if (Status.HasErrors) return null;

            _context.SaveChanges();                 
            return book;
        }

        public Book RemovePromotion(int id)
        {
            var book = _context.Find<Book>(id);
            if (book == null)
                throw new InvalidOperationException($"Could not find the book with Id of {id}.");
            book.RemovePromotion();
            _context.SaveChanges();
            return book;
        }
    }
}
