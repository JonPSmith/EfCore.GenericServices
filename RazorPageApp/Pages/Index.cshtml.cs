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
        private readonly IListBooksService _service;

        public IndexModel(IListBooksService service)
        {
            _service = service;
        }

        public BookListCombinedDto Data { get; set; }

        public void OnGet(SortFilterPageOptions options)
        {
            var bookList = _service
                .SortFilterPage(options)
                .ToList();

            Data = new BookListCombinedDto(options, bookList);
        }
    }
}
