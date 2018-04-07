using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPageApp.Helpers;
using ServiceLayer.HomeController.Dtos;

namespace RazorPageApp.Pages.Home
{
    public class ChangePubDateModel : PageModel
    {
        private readonly ICrudServices _service;

        public ChangePubDateModel(ICrudServices service)
        {
            _service = service;
        }

        [BindProperty]
        public ChangePubDateDto Dto { get; set; }

        public void OnGet(int id)
        {
            Dto = _service.ReadSingle<ChangePubDateDto>(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Dto);
            }
        }

        //There are two ways to get data. This takes the id as a parameter and picks up the other information from the [BindProperty]
        public IActionResult OnPost()
        { 
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.UpdateAndSave(Dto);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new {message = "Successfully changed publication date."});

            //Error state
            _service.CopyErrorsToModelState(ModelState, Dto);
            return Page();
        }
    }
}