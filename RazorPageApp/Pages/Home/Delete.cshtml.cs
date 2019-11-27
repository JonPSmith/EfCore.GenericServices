using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.HomeController.Dtos;
using StatusGeneric;

namespace RazorPageApp.Pages.Home
{
    public class DeleteModel : PageModel
    {
        private readonly ICrudServices _service;

        public DeleteModel(ICrudServices service)
        {
            _service = service;
        }

        [BindProperty]
        public DeleteBookDto Data { get; set; }

        public void OnGet(int id)
        {
            Data = _service.ReadSingle<DeleteBookDto>(id);
            if (!_service.IsValid)
            {
                _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
            }
        }

        public IActionResult OnPost(int id)
        {
            _service.DeleteWithActionAndSave<Book>(CheckIfInOrder, id);
            if (_service.IsValid)
                return RedirectToPage("BookUpdated", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
            return Page();
        }

        private IStatusGeneric CheckIfInOrder(DbContext context, Book entityToDelete)
        {
            var status = new StatusGenericHandler();
            //In this case the check stops an exception caused by the "DeleteBehavior.Restrict" on the BookId in the LineItem entity class
            if (context.Set<LineItem>().Any(x => x.BookId == entityToDelete.BookId))
                status.AddError("I'm sorry, but I can't delete that book because it is in a customer's order.");
            return status;
        }
    }
}