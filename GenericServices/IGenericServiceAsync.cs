using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GenericServices
{
    public interface IGenericServiceAsync : IStatusGeneric
    {
        /// <summary>
        /// This allows you to access the current DbContext that this instance of the GenericService is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        DbContext CurrentContext { get; }

        Task<T> ReadSingleAsync<T>(params object[] keys) where T : class;
        Task<T> ReadSingleAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class;
        Task ReadSingleToDtoAsync<TDto>(TDto dto, params object[] keys) where TDto : class;
        Task ReadSingleToDtoAsync<TDto>(TDto dto, Expression<Func<TDto, bool>> whereExpression) where TDto : class;
        IQueryable<T> ReadManyNoTracked<T>() where T : class;
        Task<T> AddNewAndSaveAsync<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class;
        Task UpdateAndSaveAsync<T>(T entityOrDto, string methodName = null) where T : class;
        Task DeleteAndSaveAsync<TEntity>(params object[] keys) where TEntity : class;

        Task DeleteWithActionAndSaveAsync<TEntity>(Func<DbContext, TEntity, Task<IStatusGeneric>> runBeforeDelete,
            params object[] keys) where TEntity : class;
    }
}