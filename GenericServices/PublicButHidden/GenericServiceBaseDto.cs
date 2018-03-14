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
    /// <summary>
    /// This class is simply used as a marker to find if the prvided type is a GenericServiceDto
    /// </summary>
    public abstract class GenericServiceBaseDto { }


    public class GenericServiceBaseDto<TEntity, TDto> : GenericServiceBaseDto
        where TEntity : class
        where TDto : GenericServiceBaseDto<TEntity, TDto>
    {

        protected internal virtual IQueryable<TDto> ProjectionQuery(DbContext context, IMapper mapper)
        {
            return context.Set<TEntity>().AsNoTracking().ProjectTo<TDto>(mapper);
        }



    }
}