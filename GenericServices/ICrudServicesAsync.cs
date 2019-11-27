using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace GenericServices
{
    /// <summary>
    /// This is the sync interface to CrudServicesAsync, which assumes you have one DbContext which the CrudServices setup code will register to the DbContext type
    /// You should use this with dependency injection to get an instance of the sync CrudServices
    /// </summary>
    public interface ICrudServicesAsync : IStatusGeneric
    {
        /// <summary>
        /// This allows you to access the current DbContext that this instance of the CrudServices is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// This reads async a single entity or DTO given the key(s) of the entity you want to load
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a CrudServices DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <param name="keys">The key(s) value. If there are multiple keys they must be in the correct order as defined by EF Core</param>
        /// <returns>A task with a single entity or DTO that was found by the keys. If its an entity class then it is tracked</returns>
        Task<T> ReadSingleAsync<T>(params object[] keys) where T : class;

        /// <summary>
        /// This reads async a single entity or DTO using a where clause
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a CrudServices DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <param name="whereExpression">The where expression should return a single instance, otherwise you get a </param>
        /// <returns>A task with a single entity or DTO that was found by the where clause. If its an entity class then it is tracked</returns>
        Task<T> ReadSingleAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class;

        /// <summary>
        /// This returns an <see cref="IQueryable{T}"/> result, where T can be either an actual entity class,
        /// or if a CrudServices DTO is provided then the linked entity class will be projected via AutoMapper to the DTO
        /// Apply an async method such as ToListAsync to execute the query asynchrously
        /// </summary>
        /// <typeparam name="T">This should either be an entity class or a CrudServices DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</typeparam>
        /// <returns>an <see cref="IQueryable{T}"/> result. You should apply a execute method, e.g. .ToList() or .ToListAsync() to execute the result</returns>
        IQueryable<T> ReadManyNoTracked<T>() where T : class;

        /// <summary>
        /// This allows you to read data with a query prior to the projection to a DTO.
        /// This is useful if you want to filter the data on properties not in the final DTO.
        /// It is also useful when wanting to apply a method such as IgnoreQueryFilters
        /// </summary>
        /// <typeparam name="TEntity">This must be a entity or query class in the current DbContext</typeparam>
        /// <typeparam name="TDto">This should be a class with an <see cref="ILinkToEntity{TEntity}"/> </typeparam>
        /// <param name="query">The queryable source will come from an entity or query class in the current DbContext</param>
        /// <returns></returns>
        IQueryable<TDto> ProjectFromEntityToDto<TEntity, TDto>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query)
            where TEntity : class;

        /// <summary>
        /// This will create async a new entity in the database. If you provide class which is an entity class (i.e. in your EF Core database) then
        /// the method will add, and then call SaveChanges. If the class you provide is a CrudServices DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface
        /// it will use that to create the entity by matching the DTOs properties to either, a) a public static method, b) a public ctor, or 
        /// c) by setting the properties with public setters in the entity (using AutoMapper)
        /// </summary>
        /// <typeparam name="T">This type is found from the input instance</typeparam>
        /// <param name="entityOrDto">This should either be an instance of a entity class or a CrudServices DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</param>
        /// <param name="ctorOrStaticMethodName">Optional: you can tell GenericServices which static method, ctor or CrudValues.UseAutoMapper to use</param>
        /// <returns>It returns a task with the class you provided. It will contain the primary key defined after the database. 
        /// If its a DTO then GenericServices will have copied the keys from the entity added back into the DTO</returns>
        Task<T> CreateAndSaveAsync<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class;

        /// <summary>
        /// This will update the entity referred to by the keys in the given class instance.
        /// For a entity class instance it will check the state of the instance. If its detached it will call Update, otherwise it assumes its tracked and calls SaveChanges
        /// For a CrudServices DTO it will: 
        /// a) load the existing entity class using the primary key(s) in the DTO
        /// b) This it will look for a public method that match the DTO's properties to do the update, or if no method is found it will try to use AutoMapper to copy the data over to the e
        /// c) finally it will call SaveChanges
        /// </summary>
        /// <typeparam name="T">This type is found from the input instance</typeparam>
        /// <param name="entityOrDto">This should either be an instance of a entity class or a CrudServices DTO which has a <see cref="ILinkToEntity{TEntity}"/> interface</param>
        /// <param name="methodName">Optional: you can give the method name to be used for the update, or CrudValues.UseAutoMapper to make it use AutoMapper to update the entity.</param>
        /// <returns>Task, async</returns>
        Task UpdateAndSaveAsync<T>(T entityOrDto, string methodName = null) where T : class;

        /// <summary>
        /// This allows you to update properties with public setters using a JsonPatch <cref>http://jsonpatch.com/</cref>
        /// The keys allow you to define which entity class you want updated
        /// </summary>
        /// <typeparam name="TEntity">The entity class you want to update. It should be an entity in the DbContext you are referring to.</typeparam>
        /// <param name="patch">this is a JsonPatch containing "replace" operations</param>
        /// <param name="keys">These are the primary key(s) to access the entity</param>
        /// <returns></returns>
        Task<TEntity> UpdateAndSaveAsync<TEntity>(JsonPatchDocument<TEntity> patch, params object[] keys) where TEntity : class;

        /// <summary>
        /// This allows you to update properties with public setters using a JsonPatch <cref>http://jsonpatch.com/</cref>
        /// The whereExpression allow you to define which entity class you want updated
        /// </summary>
        /// <typeparam name="TEntity">The entity class you want to update. It should be an entity in the DbContext you are referring to.</typeparam>
        /// <param name="patch">this is a JsonPatch containing "replace" operations</param>
        /// <param name="whereExpression">This is a filter command that will return a single entity (or no entity)</param>
        /// <returns></returns>
        Task<TEntity> UpdateAndSaveAsync<TEntity>(JsonPatchDocument<TEntity> patch, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class;


        /// <summary>
        /// This will delete async the entity class with the given primary key
        /// </summary>
        /// <typeparam name="TEntity">The entity class you want to delete. It should be an entity in the DbContext you are referring to.</typeparam>
        /// <param name="keys">The key(s) value. If there are multiple keys they must be in the correct order as defined by EF Core</param>
        /// <returns>Task, async</returns>
        Task DeleteAndSaveAsync<TEntity>(params object[] keys) where TEntity : class;

        /// <summary>
        /// This will find entity class async with the given primary key, then call the method you provide before calling the Remove method + SaveChanges.
        /// Your method has access to the database and can handle any relationships, and returns an <see cref="IStatusGeneric"/>. The Remove will 
        /// only go ahead if the status your method returns is Valid, i.e. no errors
        /// NOTE: This method ignore any query filters when deleting. If you are working in a multi-tenant system you should include a test
        /// that the entity you are deleting has the correct TenantId
        /// </summary>
        /// <typeparam name="TEntity">The entity class you want to delete. It should be an entity in the DbContext you are referring to.</typeparam>
        /// <param name="runBeforeDelete">You provide an async method, which is called after the entity to delete has been loaded, but before the Remove method is called.
        /// Your method has access to the database and can handle any relationships, and returns an <see cref="IStatusGeneric"/>. The Remove will 
        /// only go ahead if the status your method returns is Valid, i.e. no errors</param>
        /// <param name="keys">The key(s) value. If there are multiple keys they must be in the correct order as defined by EF Core</param>
        /// <returns>Task, async</returns>
        Task DeleteWithActionAndSaveAsync<TEntity>(Func<DbContext, TEntity, Task<IStatusGeneric>> runBeforeDelete,
            params object[] keys) where TEntity : class;
    }
}