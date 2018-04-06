using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageApp.Pages.Home
{
    public class BookUpdatedModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet(string message)
        {
            Message = message;
        }
    }
}