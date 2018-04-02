using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.Dtos;
using ServiceLayer.HomeController.QueryObjects;

namespace RazorPageApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IListBooksService _listService;
        private readonly IBookFilterDropdownService _filterService;

        public IndexModel(IListBooksService listService, IBookFilterDropdownService filterService)
        {
            _listService = listService;
            _filterService = filterService;
        }

        public SortFilterPageOptions SortFilterPageData { get; private set; }
        public IEnumerable<BookListDto> BooksList { get; private set; }

        public void OnGet(SortFilterPageOptions options)
        {
            BooksList = _listService
                .SortFilterPage(options)
                .ToList();

            SortFilterPageData = options;
        }

        public JsonResult OnGetFilter(BooksFilterBy filterBy)
        {
            return new JsonResult(_filterService.GetFilterDropDownValues(filterBy));
        }

        //You can use this to catch the data, or have items in the paremeters of the action method
        [BindProperty(SupportsGet = true)]
        public BooksFilterBy FilterBy { get; set; }

        public JsonResult OnPostFilter(SortFilterPageOptions options)
        {
            return new JsonResult(_filterService.GetFilterDropDownValues(options.FilterBy));
        }
    }
}
