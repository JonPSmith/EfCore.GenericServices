using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GenericLibsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Helpers;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages
{
    public class ChangePubDateModel : PageModel
    {
        private IChangePubDateService _service;

        public ChangePubDateModel(IChangePubDateService service)
        {
            _service = service;
        }

        [BindProperty]
        public ChangePubDateDto Dto { get; set; }

        public IStatusGeneric Status => _service;

        public IActionResult OnGet(int id)
        {
            Dto = _service.GetOriginal(id);
            if (_service.HasErrors)
            {
                Status.CopyErrorsToModelState(ModelState, Dto);
            }
            return Page();
        }

        //There are two ways to get data. This takes the id as a parameter and picks up the other information from the [BindProperty]
        public IActionResult OnPost(int id)
        { 
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.UpdateBook(id, Dto);
            if (_service.HasErrors)
            {
                Status.CopyErrorsToModelState(ModelState, Dto);
                return Page();
            }

            return RedirectToPage("BookUpdated", new { message = "Successfully changed publication date."});
        }
    }
}