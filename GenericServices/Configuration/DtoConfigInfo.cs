// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.Configuration
{
    public abstract class DtoConfigInfo<TDto,TEntity> where TDto : class where TEntity : class
    {
        public virtual Action<IMappingExpression<TEntity, TDto>> AlterReadMapping { get { return null; } }
        public virtual Action<IMappingExpression<TEntity, TDto>> AlterSaveMapping { get { return null; } }
    }
}