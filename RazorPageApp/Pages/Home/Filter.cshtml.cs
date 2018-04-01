using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.QueryObjects;

namespace RazorPageApp.Pages.Home
{
    public class FilterModel : PageModel
    {
        private readonly IBookFilterDropdownService _filterDropDownService;

        public FilterModel(IBookFilterDropdownService filterDropDownService)
        {
            _filterDropDownService = filterDropDownService;
        }

        public JsonResult OnGet(BooksFilterBy filterBy)
        {
            return new JsonResult(_filterDropDownService.GetFilterDropDownValues(filterBy));
        }

        //You can use this to catch the data, or have items in the paremeters of the action method
        [BindProperty(SupportsGet = true)]
        public BooksFilterBy FilterBy { get; set; }

        public JsonResult OnPost(SortFilterPageOptions options)
        {
            return new JsonResult(_filterDropDownService.GetFilterDropDownValues(options.FilterBy));
        }
    }
}