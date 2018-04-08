using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages.Home
{
    public class CreateModel : PageModel
    {
        private readonly ICrudServices _service;

        public CreateModel(ICrudServices service)
        {
            _service = service;
        }

        [BindProperty]
        public CreateBookDto Data { get; set; }

        public void OnGet()
        {
            Data = new CreateBookDto();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //Now I need to set up the Authors collection
            Data.SetupAuthorsCollection(_service.Context);
            _service.CreateAndSave(Data);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new { message = _service.Message});

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data);
            return Page();
        }
    }
}