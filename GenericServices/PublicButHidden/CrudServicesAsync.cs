// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using GenericServices.Configuration.Internal;
using GenericServices.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.LinqBuilders;
using GenericServices.Internal.MappingCode;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

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
        /// <param name="configAndMapper"></param>
        public CrudServicesAsync(DbContext context, IWrappedConfigAndMapper configAndMapper) : base(context, configAndMapper)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context),
                    "The DbContext class is null. Either you haven't registered GenericServices, " +
                    "or you are using the multi-DbContext version, in which case you need to use the CrudServices<TContext> and specify which DbContext to use.");
        }
    }

    /// <summary>
    /// This is the async version of GenericServices' CRUD for use in an application that has multiple DbContext
    /// You need to define the DbContext you need to carry out the CRUD actions 
    /// You should use this with dependency injection to get an instance of the sync CrudServices
    /// </summary>
    public class CrudServicesAsync<TContext> :
        //StatusGenericHandler,
        ICrudServicesAsync<TContext> where TContext : DbContext
    {
        private readonly IWrappedConfigAndMapper _configAndMapper;
        private readonly TContext _context;

        /// <summary>
        /// This allows you to access the current DbContext that this instance of the CrudServices is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        public CrudServicesAsync(TContext context, IWrappedConfigAndMapper configAndMapper)
        {
            _context = context;
            _configAndMapper = configAndMapper ?? throw new ArgumentNullException(nameof(configAndMapper));
        }

        /// <inheritdoc />
        public DbContext Context => _context;

        /// <inheritdoc />
        public async Task<IStatusGeneric<T>> ReadSingleAsync<T>(params object[] keys) where T : class
        {
            var status = new StatusGenericHandler<T>();

            T result;
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            if (entityInfo.EntityStyle == EntityStyles.HasNoKey)
                throw new InvalidOperationException($"The class {entityInfo.EntityType.Name} of style {entityInfo.EntityStyle} cannot be used in a Find.");
            if (entityInfo.EntityType == typeof(T))
            {
                result = await _context.Set<T>().FindAsync(keys).ConfigureAwait(false);
            }
            else
            {
                //else its a DTO, so we need to project the entity to the DTO and select the single element
                var projector = new CreateMapper(_context, _configAndMapper, typeof(T), entityInfo);
                result = await ((IQueryable<T>)projector.Accessor.GetViaKeysWithProject(keys))
                    .SingleOrDefaultAsync().ConfigureAwait(false);
            }

            if (result != null) return status.SetResult(result);

            if (_configAndMapper.Config.NoErrorOnReadSingleNull)
                status.Message = $"The {entityInfo.EntityType.GetNameForClass()} was not found.";
            else
                status.AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");

            return status.SetResult(null);
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric<T>> ReadSingleAsync<T>(Expression<Func<T, bool>> whereExpression) where T : class
        {
            var status = new StatusGenericHandler<T>();

            T result;
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                result = await _context.Set<T>().Where(whereExpression).SingleOrDefaultAsync().ConfigureAwait(false);
            }
            else
            {
                //else its a DTO, so we need to project the entity to the DTO and select the single element
                var projector = new CreateMapper(_context, _configAndMapper, typeof(T), entityInfo);
                result = await ((IQueryable<T>)projector.Accessor.ProjectAndThenApplyWhereExpression(whereExpression))
                    .SingleOrDefaultAsync().ConfigureAwait(false);
            }

            if (result != null) return status.SetResult(result);

            if (_configAndMapper.Config.NoErrorOnReadSingleNull)
                status.Message = $"The {entityInfo.EntityType.GetNameForClass()} was not found.";
            else
                status.AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");

            return status.SetResult(null);
        }

        /// <inheritdoc />
        public IQueryable<T> ReadManyNoTracked<T>() where T : class
        {
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                return _context.Set<T>().AsNoTracking();
            }

            //else its a DTO, so we need to project the entity to the DTO 
            var projector = new CreateMapper(_context, _configAndMapper, typeof(T), entityInfo);
            return projector.Accessor.GetManyProjectedNoTracking();
        }

        /// <inheritdoc />
        public IQueryable<TDto> ProjectFromEntityToDto<TEntity, TDto>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(query);
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
            return query(_context.Set<TEntity>())
                .ProjectTo<TDto>(_configAndMapper.MapperReadConfig);
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric<T>> CreateAndSaveAsync<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class
        {
            var status = new StatusGenericHandler<T>();

            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            entityInfo.CheckCanDoOperation(CrudTypes.Create);
            status.Message = $"Successfully created a {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType == typeof(T))
            {
                _context.Add(entityOrDto);
                status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                    _configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config).ConfigureAwait(false));
            }
            else
            {
                var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                var creator = new EntityCreateHandler<T>(dtoInfo, entityInfo, _configAndMapper, _context);
                var entity = creator.CreateEntityAndFillFromDto(entityOrDto, ctorOrStaticMethodName);
                status.CombineStatuses(creator);
                if (status.IsValid)
                {
                    _context.Add(entity);
                    status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                        dtoInfo.ShouldValidateOnSave(_configAndMapper.Config), _configAndMapper.Config));
                    if (status.IsValid)
                        entity.CopyBackKeysFromEntityToDtoIfPresent(entityOrDto, entityInfo);
                }
            }
            return status.SetResult(status.IsValid ? entityOrDto : null);
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric> UpdateAndSaveAsync<T>(T entityOrDto, string methodName = null) where T : class
        {
            var status = new StatusGenericHandler();

            if (entityOrDto is IEnumerable)
                throw new NotSupportedException($"When passing a collection to {nameof(UpdateAndSaveAsync)}, the entity/DTO type must be specified in the generic type parameter to ensure the correct overload is invoked.");

            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            entityInfo.CheckCanDoOperation(CrudTypes.Update);
            status.Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType == typeof(T))
            {
                if (!_context.Entry(entityOrDto).IsKeySet)
                    throw new InvalidOperationException($"The primary key was not set on the entity class {typeof(T).Name}. For an update we expect the key(s) to be set (otherwise it does a create).");
                if (_context.Entry(entityOrDto).State == EntityState.Detached)
                    _context.Update(entityOrDto);
                status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                    _configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config).ConfigureAwait(false));
            }
            else
            {
                var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                var updater = new EntityUpdateHandler<T>(dtoInfo, entityInfo, _configAndMapper, _context);
                status.CombineStatuses(await updater.ReadEntityAndUpdateViaDtoAsync(entityOrDto, methodName).ConfigureAwait(false));
                if (status.IsValid)
                    status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                        dtoInfo.ShouldValidateOnSave(_configAndMapper.Config), _configAndMapper.Config));
            }

            return status;
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric> UpdateAndSaveAsync<T>(IEnumerable<T> entitiesOrDtos, string methodName = null) where T : class
        {
            ArgumentNullException.ThrowIfNull(entitiesOrDtos);

            var status = new StatusGenericHandler();

            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            entityInfo.CheckCanDoOperation(CrudTypes.Update);
            status.Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType == typeof(T))
            {
                foreach (var entityOrDto in entitiesOrDtos)
                {
                    if (!_context.Entry(entityOrDto).IsKeySet)
                        throw new InvalidOperationException($"The primary key was not set on the entity class {typeof(T).Name}. For an update we expect the key(s) to be set (otherwise it does a create).");
                    if (_context.Entry(entityOrDto).State == EntityState.Detached)
                        _context.Update(entityOrDto);
                }
                status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                    _configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config).ConfigureAwait(false));
            }
            else
            {
                var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                var updater = new EntityUpdateHandler<T>(dtoInfo, entityInfo, _configAndMapper, _context);
                foreach (var entityOrDto in entitiesOrDtos)
                {
                    status.CombineStatuses(await updater.ReadEntityAndUpdateViaDtoAsync(entityOrDto, methodName).ConfigureAwait(false));
                    if (!status.IsValid)
                        break;
                }
                if (status.IsValid)
                    status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                        dtoInfo.ShouldValidateOnSave(_configAndMapper.Config), _configAndMapper.Config));
            }

            return status;
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric<TEntity>> UpdateAndSaveAsync<TEntity>(JsonPatchDocument<TEntity> patch, params object[] keys) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(patch);
            return await LocalUpdateAndSaveAsync(patch, async () => await _context.FindAsync<TEntity>(keys).ConfigureAwait(false));
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric<TEntity>> UpdateAndSaveAsync<TEntity>(JsonPatchDocument<TEntity> patch, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(patch);
            return await LocalUpdateAndSaveAsync(patch, () => _context.Set<TEntity>().SingleOrDefaultAsync(whereExpression)).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric> DeleteAndSaveAsync<TEntity>(params object[] keys) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(keys);

            var status = new StatusGenericHandler();

            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
            entityInfo.CheckCanDoOperation(CrudTypes.Delete);
            status.Message = $"Successfully deleted a {ExtractDisplayHelpers.GetNameForClass<TEntity>()}";

            var whereWithKeys = entityInfo.PrimaryKeyProperties.CreateFilter<TEntity>(keys);
            var entity = await _context.Set<TEntity>().SingleOrDefaultAsync(whereWithKeys);
            if (entity == null)
            {
                status.AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
                return status;
            }
            _context.Remove(entity);
            status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                _configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config).ConfigureAwait(false));

            return status;
        }

        /// <inheritdoc />
        public async Task<IStatusGeneric> DeleteWithActionAndSaveAsync<TEntity>(Func<DbContext, TEntity, Task<IStatusGeneric>> runBeforeDelete,
            params object[] keys) where TEntity : class
        {
            ArgumentNullException.ThrowIfNull(runBeforeDelete);
            ArgumentNullException.ThrowIfNull(keys);

            var status = new StatusGenericHandler();

            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
            entityInfo.CheckCanDoOperation(CrudTypes.Delete);
            status.Message = $"Successfully deleted a {ExtractDisplayHelpers.GetNameForClass<TEntity>()}";

            var whereWithKeys = entityInfo.PrimaryKeyProperties.CreateFilter<TEntity>(keys);
            var entity = await _context.Set<TEntity>().IgnoreQueryFilters().SingleOrDefaultAsync(whereWithKeys);
            if (entity == null)
            {
                status.AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
                return status;
            }

            status.CombineStatuses(await runBeforeDelete(_context, entity).ConfigureAwait(false));
            if (!status.IsValid) return status;

            _context.Remove(entity);
            status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                _configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config).ConfigureAwait(false));

            return status;
        }

        /// <summary>
        /// Local version of UpdateAndSave with JsonPatch - contains the common code
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="patch"></param>
        /// <param name="getEntity"></param>
        /// <returns></returns>
        private async Task<IStatusGeneric<TEntity>> LocalUpdateAndSaveAsync<TEntity>(JsonPatchDocument<TEntity> patch, Func<Task<TEntity>> getEntity)
            where TEntity : class
        {
            var status = new StatusGenericHandler<TEntity>();

            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
            entityInfo.CheckCanDoOperation(CrudTypes.Update);
            status.Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType != typeof(TEntity))
                throw new NotImplementedException(
                    $"I could not find the entity class {typeof(TEntity).Name}. JsonPatch only works on entity classes.");

            var entity = await getEntity().ConfigureAwait(false);
            if (entity != null)
                patch.ApplyTo(entity, error => status.AddError(error.ErrorMessage));
            else
                status.AddError(
                    $"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were trying to update.");
            if (status.IsValid)
                status.CombineStatuses(await _context.SaveChangesWithOptionalValidationAsync(
                    _configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config).ConfigureAwait(false));

            return status.SetResult(entity);
        }
    }
}