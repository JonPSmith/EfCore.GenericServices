using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.QueryObjects;
using ServiceLayer.HomeController.Services;

namespace RazorPageApp.Pages.Home
{
    public class FilterModel : PageModel
    {
        private readonly IBookFilterDropdownService _filterService;

        public FilterModel(EfCoreContext context)
        {
            _filterService = new BookFilterDropdownService(context);
        }

        public JsonResult OnGet(BooksFilterBy filterBy)
        {
            return new JsonResult(_filterService.GetFilterDropDownValues(filterBy));
        }

        //You can use this to catch the data, or have items in the parameters of the action method
        [BindProperty(SupportsGet = true)]
        public BooksFilterBy FilterBy { get; set; }

        public JsonResult OnPost(SortFilterPageOptions options)
        {
            return new JsonResult(_filterService.GetFilterDropDownValues(options.FilterBy));
        }
    }
}