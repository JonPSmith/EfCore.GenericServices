using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages.Authors
{
    public class EditModel : PageModel
    {
        private readonly ICrudServices _service;

        public EditModel(ICrudServices service)
        {
            _service = service;
        }

        [BindProperty]
        public Author Data { get; set; }

        public void OnGet(int id)
        {
            Data = _service.ReadSingle<Author>(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
            }
        }

        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.UpdateAndSave(Data);
            if (_service.IsValid)
                return RedirectToPage("Index", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
            return Page();
        }
    }
}