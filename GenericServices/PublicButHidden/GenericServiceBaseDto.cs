// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using GenericLibsBase;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.PublicButHidden
{
    public class GenericServiceBaseDto<TEntity, TDto, TContext>
        where TEntity : class
        where TDto : GenericServiceBaseDto<TEntity, TDto, TContext>
        where TContext : DbContext
    {

        internal IStatusGeneric Setup(TContext context)
        {
            throw new NotImplementedException();
        }

        protected internal virtual IQueryable<TDto> ProjectionQuery(TContext context, IMapper mapper)
        {
            return context.Set<TEntity>().AsNoTracking().ProjectTo<TDto>(mapper);
        }



    }
}