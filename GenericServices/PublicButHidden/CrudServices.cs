// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using GenericServices.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using Microsoft.EntityFrameworkCore;
using StackExchange.Profiling;

namespace GenericServices.PublicButHidden
{
    /// <summary>
    /// This is the sync version of GenericServices' CRUD, which assumes you have one DbContext which the CrudServices setup code will register to the DbContext type
    /// You should use this with dependency injection to get an instance of the sync CrudServices
    /// </summary>
    public class CrudServices : CrudServices<DbContext>, ICrudServices
    {
        public static MiniProfiler Profiler { get; set; }

        /// <summary>
        /// CrudServices needs the correct DbContext and the AutoMapper config
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wapper"></param>
        public CrudServices(DbContext context, IWrappedAutoMapperConfig wapper) : base(context, wapper)
        {
            if (context == null)
                throw new ArgumentNullException("The DbContext class is null. Either you haven't registered GenericServices, " +
                     "or you are using the multi-DbContext version, in which case you need to use the CrudServices<TContext> and specify which DbContext to use.");
        }
    }

    /// <summary>
    /// This is the sync version of GenericServices' CRUD for use in an application that has multiple DbContext
    /// You need to define the DbContext you need to carry out the CRUD actions 
    /// You should use this with dependency injection to get an instance of the sync CrudServices
    /// </summary>
    public class CrudServices<TContext> : 
        StatusGenericHandler, 
        ICrudServices<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IWrappedAutoMapperConfig _wrapperMapperConfigs;

        /// <inheritdoc />
        public DbContext Context => _context;

        /// <summary>
        /// CrudServices needs the correct DbContext and the AutoMapper config
        /// </summary>
        /// <param name="context"></param>
        /// <param name="wapper"></param>
        public CrudServices(TContext context, IWrappedAutoMapperConfig wapper)
        {
            _context = context;
            _wrapperMapperConfigs = wapper ?? throw new ArgumentException(nameof(wapper));
        }

        /// <inheritdoc />
        public T ReadSingle<T>(params object[] keys) where T : class
        {    
            T result;
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                result = _context.Set<T>().Find(keys);
            }
            else
            {
                //else its a DTO, so we need to project the entity to the DTO and select the single element
                var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
                result = ((IQueryable<T>) projector.Accessor.GetViaKeysWithProject(keys)).SingleOrDefault();
            }

            if (result == null)
            {
                AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");
            }

            return result;
        }

        /// <inheritdoc />
        public T ReadSingle<T>(Expression<Func<T, bool>> whereExpression) where T : class
        {
            T result;
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                result = _context.Set<T>().Where(whereExpression).SingleOrDefault();
            }
            else
            {
                //else its a DTO, so we need to project the entity to the DTO and select the single element
                var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
                result = ((IQueryable<T>)projector.Accessor.ProjectAndThenApplyWhereExpression(whereExpression)).SingleOrDefault();
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
            Message = $"Successfully read many {ExtractDisplayHelpers.GetNameForClass<T>()}";
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                return _context.Set<T>().AsNoTracking();
            }

            //else its a DTO, so we need to project the entity to the DTO 
            var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
            return projector.Accessor.GetManyProjectedNoTracking();
        }

        /// <inheritdoc />
        public T CreateAndSave<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class
        {
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            Message = $"Successfully created a {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType == typeof(T))
            {
                _context.Add(entityOrDto);
                _context.SaveChanges();
            }
            else
            {
                DecodedDto dtoInfo;                
                using (CrudServices.Profiler.Step("DTO lookup"))
                {
                    dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                }
                EntityCreateHandler<T> creator;
                using (CrudServices.Profiler.Step("create EntityCreateHander"))
                {
                    creator = new EntityCreateHandler<T>(dtoInfo, entityInfo, _wrapperMapperConfigs, _context);
                }
                object entity;
                using (CrudServices.Profiler.Step("CreateEntityAndFillFromDto"))
                {
                    entity = creator.CreateEntityAndFillFromDto(entityOrDto, ctorOrStaticMethodName);
                }
                CombineStatuses(creator);
                if (IsValid)
                {
                    using (CrudServices.Profiler.Step("_context.Add"))
                    {
                        _context.Add(entity);
                    }
                    CombineStatuses(_context.SaveChangesWithOptionalValidation(dtoInfo.ValidateOnSave));
                    using (CrudServices.Profiler.Step("CopyBackKeysFromEntityToDtoIfPresent"))
                    {
                        if (IsValid)
                            entity.CopyBackKeysFromEntityToDtoIfPresent(entityOrDto, entityInfo);
                    }
                }
            }
            using (CrudServices.Profiler.Step("return"))
            {
                return IsValid ? entityOrDto : null;
            }
            
        }

        /// <inheritdoc />
        public void UpdateAndSave<T>(T entityOrDto, string methodName = null) where T : class
        {
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
            Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType == typeof(T))
            {
                if (!_context.Entry(entityOrDto).IsKeySet)
                    throw new InvalidOperationException($"The primary key was not set on the entity class {typeof(T).Name}. For an update we expect the key(s) to be set (otherwise it does a create).");
                if (_context.Entry(entityOrDto).State == EntityState.Detached)
                    _context.Update(entityOrDto);
                _context.SaveChanges();
            }
            else
            {
                DecodedDto dtoInfo;
                using (CrudServices.Profiler.Step("DTO lookup"))
                {
                    dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                }

                EntityUpdateHandler<T> updater;
                using (CrudServices.Profiler.Step("Create EntityUpdateHandler"))
                {
                    updater = new EntityUpdateHandler<T>(dtoInfo, entityInfo, _wrapperMapperConfigs, _context);
                }

                using (CrudServices.Profiler.Step("Create ReadEntityAndUpdateViaDto"))
                {
                    CombineStatuses(updater.ReadEntityAndUpdateViaDto(entityOrDto, methodName));
                }
                using (CrudServices.Profiler.Step("Create SaveChangesWithOptionalValidation"))
                {
                    if (IsValid)
                        CombineStatuses(_context.SaveChangesWithOptionalValidation(dtoInfo.ValidateOnSave));
                }
            }
        }

        /// <inheritdoc />
        public void DeleteAndSave<TEntity>(params object[] keys) where TEntity : class
        {
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
            Message = $"Successfully deleted a {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType != typeof(TEntity))
                throw new NotImplementedException(
                    "You cannot delete a DTO/ViewModel. You must provide a real entity class.");

            var entity = _context.Set<TEntity>().Find(keys);
            if (entity == null)
            {
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
            }
            if (!IsValid) return;

            _context.Remove(entity);
            _context.SaveChanges();
        }

        /// <inheritdoc />
        public void DeleteWithActionAndSave<TEntity>(Func<DbContext, TEntity, IStatusGeneric> runBeforeDelete,
            params object[] keys) where TEntity : class
        {
            var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
            Message = $"Successfully deleted a {entityInfo.EntityType.GetNameForClass()}";
            if (entityInfo.EntityType != typeof(TEntity))
                throw new NotImplementedException(
                    "You cannot delete a DTO/ViewModel. You must provide a real entity class.");

            var entity = _context.Set<TEntity>().Find(keys);
            if (entity == null)
            {
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
                return;
            }

            CombineStatuses(runBeforeDelete(_context, entity));
            if (!IsValid) return;

            _context.Remove(entity);
            _context.SaveChanges();
        }

    }
}