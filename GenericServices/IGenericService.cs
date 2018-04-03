// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.


using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericServices
{
    /// <summary>
    /// This is the sync interface to GenericService, which assumes you have one DbContext which the GenericService setup code will register to the DbContext type
    /// You should use this with dependency injection to get an instance of the sync GenericService
    /// </summary>
    public interface IGenericService : IStatusGeneric
    {
        /// <summary>
        /// This allows you to access the current DbContext that this instance of the GenericService is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        DbContext CurrentContext { get; }

        /// <summary>
        /// This reads a single entity or DTO given the key(s) of the entity you want to load
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a GenericService DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <param name="keys">The key(s) value. If there are multiple keys they must be in the correct order as defined by EF Core</param>
        /// <returns></returns>
        T ReadSingle<T>(params object[] keys) where T : class;

        /// <summary>
        /// This reads a single entity or DTO using a where clause
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a GenericService DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <param name="whereExpression">The where expression should return a single instance, otherwise you get a </param>
        /// <returns></returns>
        T ReadSingle<T>(Expression<Func<T, bool>> whereExpression) where T : class;
        void ReadSingleToDto<TDto>(TDto dto, params object[] keys) where TDto : class;
        void ReadSingleToDto<TDto>(TDto dto, Expression<Func<TDto, bool>> whereExpression) where TDto : class;
        IQueryable<T> ReadManyNoTracked<T>() where T : class;
        T AddNewAndSave<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class;
        void UpdateAndSave<T>(T entityOrDto, string methodName = null) where T : class;
        void DeleteAndSave<TEntity>(params object[] keys) where TEntity : class;

        void DeleteWithActionAndSave<TEntity>(Func<DbContext, TEntity, IStatusGeneric> runBeforeDelete,
            params object[] keys) where TEntity : class;
    }
}