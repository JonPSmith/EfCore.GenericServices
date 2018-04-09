using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageApp.Pages.Authors
{
    public class CreateModel : PageModel
    {
        private readonly ICrudServices _service;

        public CreateModel(ICrudServices service)
        {
            _service = service;
        }

        [BindProperty]
        public Author Data { get; set; }

        public void OnGet(int id)
        {
            Data = new Author();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.CreateAndSave(Data);
            if (_service.IsValid)
                return RedirectToPage("Index", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data, "Data");
            return Page();
        }
    }
}