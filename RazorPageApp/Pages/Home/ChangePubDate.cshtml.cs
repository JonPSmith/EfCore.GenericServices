using GenericServices;
using RazorPageApp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public ChangePubDateDto Data { get; set; }

        public void OnGet(int id)
        {
            Data = _service.ReadSingle<ChangePubDateDto>(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
            }
        }

        //There are two ways to get data. This takes the id as a parameter and picks up the other information from the [BindProperty]
        public IActionResult OnPost()
        { 
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _service.UpdateAndSave(Data);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new {message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
            return Page();
        }
    }
}