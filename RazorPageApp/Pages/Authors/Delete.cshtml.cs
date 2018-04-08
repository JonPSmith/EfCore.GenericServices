using System.Linq;
using DataLayer.EfClasses;
using GenericServices;
using GenericServices.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPageApp.Pages.Authors
{
    public class DeleteModel : PageModel
    {
        private readonly ICrudServices _service;

        public DeleteModel(ICrudServices service)
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
                _service.CopyErrorsToModelState(ModelState, Data, "Data");
            }
        }

        public IActionResult OnPost(int id)
        {
            _service.DeleteWithActionAndSave<Author>(CheckIfInOrder, id);
            if (_service.IsValid)
                return RedirectToPage("Index", new { message = _service.Message });

            //Error state
            _service.CopyErrorsToModelState(ModelState, Data, "Data");
            return Page();
        }

        private IStatusGeneric CheckIfInOrder(DbContext context, Author entityToDelete)
        {
            var status = new StatusGenericHandler();
            //Nothing in the system would stop an author from being deleted, but that would cause problems to books where they are an author
            //Therefore you check if the author is linked to any books via the many-to-many linking table, BookAuthor
            if (context.Set<BookAuthor>().Any(x => x.AuthorId == entityToDelete.AuthorId))
                status.AddError("I'm sorry, but I can't delete that author because they are in existing books.");

            //By providing a message here it will override the normal success message provided by GenericServices
            //NOTE: If the status has an error then this message will be replaced with a "there were errors" message
            status.Message = $"Successfully deleted the author {entityToDelete.Name}";
            return status;
        }
    }
}