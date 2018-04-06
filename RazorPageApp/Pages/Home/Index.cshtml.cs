using System.Collections.Generic;
using DataLayer.QueryObjects;
using GenericServices;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.Dtos;
using ServiceLayer.HomeController.QueryObjects;

namespace RazorPageApp.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly ICrudServices _service;

        public IndexModel(ICrudServices service)
        {
            _service = service;
        }

        public SortFilterPageOptions SortFilterPageData { get; private set; }
        public IEnumerable<BookListDto> BooksList { get; private set; }

        public void OnGet(SortFilterPageOptions options)
        {
            var booksQuery = _service.ReadManyNoTracked<BookListDto>()
                .OrderBooksBy(options.OrderByOptions)
                .FilterBooksBy(options.FilterBy,
                    options.FilterValue);

            options.SetupRestOfDto(booksQuery);

            BooksList = booksQuery.Page(options.PageNum - 1, options.PageSize);
            SortFilterPageData = options;
        }
    }
}
