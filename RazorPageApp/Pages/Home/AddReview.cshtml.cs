using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Helpers;
using ServiceLayer.HomeController;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages
{
    public class AddReviewModel : PageModel
    {
        private readonly IAddReviewService _service;

        public AddReviewModel(IAddReviewService service)
        {
            _service = service;
        }

        [BindProperty]
        public AddReviewDto Dto { get; set; }

        public void OnGet(int id)
        {
            Dto = _service.GetOriginal(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Dto);
            }
        }

        //There are two ways to get data. This takes the id as a parameter and picks up the other information from the [BindProperty]
        public IActionResult OnPost(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.AddReviewToBook(Dto);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new { message = "Successfully added a review to the book." });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Dto);
            return Page();
        }
    }
}