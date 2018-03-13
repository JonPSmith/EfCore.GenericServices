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

        public IActionResult OnGet(int id)
        {
            Dto = _service.GetOriginal(id);
            if (_service.HasErrors)
            {
                _service.CopyErrorsToModelState(ModelState, Dto, nameof(Dto));
            }
            return Page();
        }

        //There are two ways to get data. This has the dto as an parameter (rather than have [BindProperty] of the public Dto)
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.AddPromotion(Dto);
            if (!_service.HasErrors)
                return RedirectToPage("BookUpdated", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Dto, nameof(Dto));
            return Page();
        }
    }
}