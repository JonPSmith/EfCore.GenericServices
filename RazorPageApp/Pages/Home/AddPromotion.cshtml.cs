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
    public class AddPromotionModel : PageModel
    {
        private readonly IAddRemovePromotionService _service;

        public AddPromotionModel(IAddRemovePromotionService service)
        {
            _service = service;
        }

        [BindProperty]
        public AddRemovePromotionDto Dto { get; set; }

        public void OnGet(int id)
        {
            Dto = _service.GetOriginal(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Dto, nameof(Dto));
            }
        }

        //There are two ways to get data. This uses the [BindProperty] because it preserves the original data if there is an error
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.AddPromotion(Dto);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Dto, nameof(Dto));
            return Page();
        }
    }
}