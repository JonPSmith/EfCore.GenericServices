using GenericServices.Setup;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Helpers
{
    public class CreateNewDBContextHelper : ICreateNewDBContext
    {
        Func<DbContext> _context;

        public CreateNewDBContextHelper(Func<DbContext> context)
        {
            _context = context;
        }

        public DbContext CreateNew()
        {
            return _context();
        }
    }
}
