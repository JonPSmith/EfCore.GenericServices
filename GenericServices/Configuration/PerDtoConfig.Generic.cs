// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.Configuration
{
    /// <summary>
    /// This provides a per-DTO/ViewModel configuation source
    /// </summary>
    /// <typeparam name="TDto"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class PerDtoConfig<TDto,TEntity> : PerDtoConfig
        where TDto : class where TEntity : class
    {
        //-------------------------------------------------
        //Properies to alter the AutoMapper Read and Save mappings

        /// <summary>
        /// This allows you to add to the AutoMapper's read mapping, i.e. from Entity class to DTO/ViewModel
        /// For instance you can use .ForMember(...) to set a specific LINQ for certain properies - see BookListDto example
        /// </summary>
        public virtual Action<IMappingExpression<TEntity, TDto>> AlterReadMapping { get { return null; } }

        /// <summary>
        /// This allows you to alter the AutoMapper create/update mapping, i.e. from DTO/ViewModel to Entity class 
        /// </summary>
        public virtual Action<IMappingExpression<TDto, TEntity>> AlterSaveMapping { get { return null; } }

    }
}