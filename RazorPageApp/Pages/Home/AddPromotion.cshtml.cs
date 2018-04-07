using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Helpers;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages.Home
{
    public class AddPromotionModel : PageModel
    {
        private readonly ICrudServices _service;

        public AddPromotionModel(ICrudServices service)
        {
            _service = service;
        }

        [BindProperty]
        public AddRemovePromotionDto Data { get; set; }

        public void OnGet(int id)
        {
            Data = _service.ReadSingle<AddRemovePromotionDto>(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Data, "Data");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.UpdateAndSave(Data);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data, "Data");
            return Page();
        }
    }
}