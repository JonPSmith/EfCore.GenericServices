// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using GenericLibsBase;
using GenericServices.Internal;
using GenericServices.Internal.Decoders;
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
            var entityInfo = typeof(T).GetUnderlyingEntityInfo(_context);
            if (entityInfo.EntityType == typeof(T))
            {
                var result = _context.Set<T>().Find(keys);
                if (result == null)
                {
                    AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<T>()} you were looking for.");
                }
                return result;
            }

            throw new NotImplementedException();
        }

    }
}