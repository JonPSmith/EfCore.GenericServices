using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IListBooksService _listService;
        private readonly IBookFilterDropdownService _filterDropDownService;

        public IndexModel(IListBooksService listService, IBookFilterDropdownService filterDropDownService)
        {
            _listService = listService;
            _filterDropDownService = filterDropDownService;
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

        public JsonResult OnGetFilterSearchContent(SortFilterPageOptions options)
        {
            return new JsonResult(_filterDropDownService.GetFilterDropDownValues(options.FilterBy));
        }
    }
}
