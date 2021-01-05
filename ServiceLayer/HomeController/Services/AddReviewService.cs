// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.HomeController.Dtos;
using StatusGeneric;

namespace ServiceLayer.HomeController.Services
{
    public class AddReviewService : StatusGenericHandler, IAddReviewService
    {
        private readonly EfCoreContext _context;

        public AddReviewService(EfCoreContext context)
        {
            _context = context;
        }

        public AddReviewDto GetOriginal(int id) 
        {
            var dto = _context.Books
                .Select(p => new AddReviewDto
                {
                    BookId = p.BookId,
                    Title = p.Title
                })
                .SingleOrDefault(k => k.BookId == id);
            if (dto == null)
                AddError("Sorry, I could not find the book you were looking for.");
            return dto;
        }

        public Book AddReviewToBook(AddReviewDto dto)
        {
            var book = _context.Find<Book>(dto.BookId);
            if (book == null)
            {
                AddError("Sorry, I could not find the book you were looking for.");
                return null;
            }
            book.AddReview(dto.NumStars, dto.Comment, dto.VoterName, _context);
            _context.SaveChanges(); 
            return book; 
        }
    }
}
