using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.QueryObjects;
using GenericServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.AuthorController.Dtos;

namespace RazorPageApp.Pages.Author
{
    public class IndexModel : PageModel
    {

        private readonly ICrudServices _service;

        public IndexModel(ICrudServices service)
        {
            _service = service;
        }

        public int NumPages { get; set; }
        public int[] PageSizes = new[] { 5, 10, 20, 50, 100, 500, 1000 };
        [BindProperty(SupportsGet = true)]
        public int PageNum { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public IEnumerable<AuthorWithBookCountDto> AuthorList { get; private set; }

        public void OnGet()
        {
            var query = _service.ReadManyNoTracked<AuthorWithBookCountDto>()
                .OrderBy(x => x.Name).AsQueryable();
            NumPages = (int)Math.Ceiling(
                (double)query.Count() / PageSize);
            PageNum = Math.Min(
                Math.Max(1, PageNum), NumPages);

            AuthorList = query.Page(PageNum - 1, PageSize);
        }
    }
}