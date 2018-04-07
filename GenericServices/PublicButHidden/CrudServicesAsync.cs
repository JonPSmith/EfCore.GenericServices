// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GenericServices.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.PublicButHidden
{
    /// <summary>
    /// This is the async version of GenericServices' CRUD, which assumes you have one DbContext which the CrudServices setup code will register to the DbContext type
    /// You should use this with dependency injection to get an instance of the sync CrudServices
    /// </summary>
    public class CrudServicesAsync : CrudServicesAsync<DbContext>, ICrudServicesAsync
    {
        /// <summary>
        /// CrudServicesAsync needs the correct DbContext and the AutoMapper config
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wapper"></param>
        public CrudServicesAsync(DbContext context, IWrappedAutoMapperConfig wapper) : base(context, wapper)
        {
        }
    }

    /// <summary>
    /// This is the async version of GenericServices' CRUD for use in an application that has multiple DbContext
    /// You need to define the DbContext you need to carry out the CRUD actions 
    /// You should use this with dependency injection to get an instance of the sync CrudServices
    /// </summary>
    public class CrudServicesAsync<TContext> :
        StatusGenericHandler,
        ICrudServicesAsync<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IWrappedAutoMapperConfig _wrapperMapperConfigs;

        /// <inheritdoc />
        public DbContext CurrentContext => _context;

        /// <summary>
        /// This allows you to access the current DbContext that this instance of the CrudServices is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        public CrudServicesAsync(TContext context, IWrappedAutoMapperConfig wapper)
        {
            _context = context;
            _wrapperMapperConfigs = wapper ?? throw new ArgumentException(nameof(wapper));
        }

        /// <inheritdoc />
        public async Task<T> ReadSingleAsync<T>(params object[] keys) where T : class
        {
            T result;
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                result = await _context.Set<T>().FindAsync(keys).ConfigureAwait(false);
            }
            else
            {
                //else its a DTO, so we need to project the entity to the DTO and select the single element
                var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
                result = await ((IQueryable<T>) projector.Accessor.GetViaKeysWithProject(keys))
                    .SingleOrDefaultAsync().ConfigureAwait(false);
            }

            if (result == null)
            {
                AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");
            }
            return result;
        }

        /// <inheritdoc />
        public async Task<T> ReadSingleAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class
        {
            T result;
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                result = await _context.Set<T>().Where(whereExpression).SingleOrDefaultAsync().ConfigureAwait(false);
            }
            else
            {
                //else its a DTO, so we need to project the entity to the DTO and select the single element
                var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
                result = await ((IQueryable<T>)projector.Accessor.ProjectAndThenApplyWhereExpression(whereExpression))
                    .SingleOrDefaultAsync().ConfigureAwait(false);
            }

            if (result == null)
            {
                AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");
            }
            return result;
        }

        /// <inheritdoc />
        public IQueryable<T> ReadManyNoTracked<T>() where T : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                return _context.Set<T>().AsNoTracking();
            }

            //else its a DTO, so we need to project the entity to the DTO 
            var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
            return projector.Accessor.GetManyProjectedNoTracking();
        }

        /// <inheritdoc />
        public async Task<T> CreateAndSaveAsync<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                _context.Add(entityOrDto);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                var creator = new EntityCreateHandler<T>(dtoInfo, entityInfo, _wrapperMapperConfigs, _context);
                var entity = creator.CreateEntityAndFillFromDto(entityOrDto, ctorOrStaticMethodName);
                CombineStatuses(creator);
                if (IsValid)
                {
                    _context.Add(entity);
                    CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(dtoInfo.ValidateOnSave).ConfigureAwait(false));
                    if (IsValid)
                        entity.CopyBackKeysFromEntityToDtoIfPresent(entityOrDto, entityInfo);
                }
            }
            SetMessageIfNotAlreadySet($"Successfully created a {entityInfo.EntityType.GetNameForClass()}");
            return IsValid ? entityOrDto : null;
        }

        /// <inheritdoc />
        public async Task UpdateAndSaveAsync<T>(T entityOrDto, string methodName = null) where T : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                if (_context.Entry(entityOrDto).State == EntityState.Detached)
                    _context.Update(entityOrDto);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
                var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                var updater = new EntityUpdateHandler<T>(dtoInfo, entityInfo, _wrapperMapperConfigs, _context);
                CombineStatuses(updater.ReadEntityAndUpdateViaDto(entityOrDto, methodName));
                if (IsValid)
                    CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(dtoInfo.ValidateOnSave).ConfigureAwait(false));        
            }
            SetMessageIfNotAlreadySet($"Successfully updated the {entityInfo.EntityType.GetNameForClass()}");
        }

        /// <inheritdoc />
        public async Task DeleteAndSaveAsync<TEntity>(params object[] keys) where TEntity : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(TEntity));
            if (entityInfo.EntityType != typeof(TEntity))
                throw new NotImplementedException(
                    "You cannot delete a DTO/ViewModel. You must provide a real entity class.");

            var entity = await _context.Set<TEntity>().FindAsync(keys).ConfigureAwait(false);
            if (entity == null)
            {
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
                return;
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            SetMessageIfNotAlreadySet($"Successfully deleted a {ExtractDisplayHelpers.GetNameForClass<TEntity>()}");
        }

        /// <inheritdoc />
        public async Task DeleteWithActionAndSaveAsync<TEntity>(Func<DbContext, TEntity, Task<IStatusGeneric>> runBeforeDelete,
            params object[] keys) where TEntity : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(TEntity));
            if (entityInfo.EntityType != typeof(TEntity))
                throw new NotImplementedException(
                    "You cannot delete a DTO/ViewModel. You must provide a real entity class.");

            var entity = await _context.Set<TEntity>().FindAsync(keys).ConfigureAwait(false);
            if (entity == null)
            {
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
                return;
            }

            CombineStatuses(await runBeforeDelete(_context, entity).ConfigureAwait(false));
            if (!IsValid) return;

            _context.Remove(entity);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            SetMessageIfNotAlreadySet($"Successfully deleted a {ExtractDisplayHelpers.GetNameForClass<TEntity>()}");
        }

    }
}