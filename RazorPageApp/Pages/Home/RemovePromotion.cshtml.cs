using DataLayer.EfClasses;
using GenericServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Helpers;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages.Home
{
    public class RemovePromotionModel : PageModel
    {
        private readonly ICrudServices _service;

        public RemovePromotionModel(ICrudServices service)
        {
            _service = service;
        }


        [BindProperty]
        public AddRemovePromotionDto Dto { get; set; }

        public void OnGet(int id)
        {
            Dto = _service.ReadSingle<AddRemovePromotionDto>(id);
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
            _service.UpdateAndSave(Dto, nameof(Book.RemovePromotion));
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Dto, nameof(Dto));
            return Page();
        }
    }
}