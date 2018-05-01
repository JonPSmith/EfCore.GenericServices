using System.IO;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.QueryObjects;
using ServiceLayer.HomeController.Services;

namespace RazorPageApp.Pages.Home
{
    public class MyClass
    {
        public BooksFilterBy FilterBy { get; set; }

        public string MyString { get; set; }
    }

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

        public JsonResult OnPost(BooksFilterBy filterBy)
        {
            return new JsonResult(_filterService.GetFilterDropDownValues(filterBy));
        }

        //------------------------------------------
        //example of handlng json content

        //You can't have two [FromBody] items - see https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-2.1#bind-formatted-data-from-the-request-body
        //[FromBody]
        //public MyClass LocalClass { get; set; }

        //public JsonResult OnPost([FromBody]MyClass myClass)
        //{
        //    return new JsonResult(_filterService.GetFilterDropDownValues(myClass.FilterBy));
        //}


    }
}