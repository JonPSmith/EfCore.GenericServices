using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Dto = _service.GetOriginal((int)id);
            if (Dto == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (id == null)
            {
                return NotFound();
            }
            _service.UpdateBook((int)id, Dto);

            return RedirectToPage("BookUpdated", new { message = "Successfully changed publication date"});
        }
    }
}