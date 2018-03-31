// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.


using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace GenericServices
{
    public interface IGenericService : IStatusGeneric
    {
        /// <summary>
        /// This allows you to access the current DbContext that this instance of the GenericService is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        DbContext CurrentContext { get; }

        T ReadSingle<T>(params object[] keys) where T : class;
        T ReadSingle<T>(Expression<Func<T, bool>> whereExpression) where T : class;
        IQueryable<T> ReadManyNoTracked<T>() where T : class;
        T AddNewAndSave<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class;
        void UpdateAndSave<T>(T entityOrDto, string methodName = null) where T : class;
        void DeleteAndSave<TEntity>(params object[] keys) where TEntity : class;

        void DeleteWithActionAndSave<TEntity>(Func<DbContext, TEntity, IStatusGeneric> runBeforeDelete,
            params object[] keys) where TEntity : class;
    }
}