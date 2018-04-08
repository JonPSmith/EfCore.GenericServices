using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GenericServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPageApp.Pages.Authors
{
    public class EditModel : PageModel
    {
        private readonly ICrudServices _service;

        public EditModel(ICrudServices service)
        {
            _service = service;
        }


        public void OnGet()
        {

        }
    }
}