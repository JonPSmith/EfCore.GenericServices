// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using DataLayer.QueryObjects;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController.Dtos;
using ServiceLayer.HomeController.QueryObjects;

namespace ServiceLayer.HomeController.Services
{
    public class ListBooksService : IListBooksService
    {
        private readonly EfCoreContext _context;

        public ListBooksService(EfCoreContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<BookListDto>> SortFilterPage
            (SortFilterPageOptions options)
        {
            var booksQuery = _context.Books            
                .AsNoTracking()                        
                .MapBookToDto()                        
                .OrderBooksBy(options.OrderByOptions)  
                .FilterBooksBy(options.FilterBy,       
                               options.FilterValue);   

            await options.SetupRestOfDto(booksQuery);        

            return booksQuery.Page(options.PageNum-1,  
                                   options.PageSize);  
        }
    }

}