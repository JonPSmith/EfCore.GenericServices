// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.Configuration
{
    public abstract class DtoConfigInfo<TDto,TEntity> where TDto : class where TEntity : class
    {
        /// <summary>
        /// This allows you to add to the AutoMapper's read mapping, i.e. Entity classes to DTO/ViewModel
        /// For instance you can use .ForMember(...) to set a specific 
        /// </summary>
        public virtual Action<IMappingExpression<TEntity, TDto>> AlterReadMapping { get { return null; } }
        public virtual Action<IMappingExpression<TDto, TEntity>> AlterSaveMapping { get { return null; } }

        public virtual bool UseSaveChangesWithValidation { get; } = false;


    }
}