// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericLibsBase;
using GenericServices.Internal;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.PublicButHidden
{
    public class GenericService<TContext> : StatusGenericHandler where TContext : DbContext
    {
        private readonly TContext _context;

        public GenericService(TContext context)
        {
            _context = context;
        }

        public T GetSingle<T>(params object[] keys) where T : class
        {
            var result = _context.Set<T>().Find(keys);
            if (result == null)
            {
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<T>()} you were looking for.");
            }

            return result;
        }
    }
}