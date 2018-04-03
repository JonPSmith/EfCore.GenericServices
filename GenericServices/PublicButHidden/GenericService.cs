// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using GenericServices.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.PublicButHidden
{
    public class GenericService : GenericService<DbContext>, IGenericService
    {
        public GenericService(DbContext context, IWrappedAutoMapperConfig wapper) : base(context, wapper)
        {
        }
    }

    public class GenericService<TContext> : 
        StatusGenericHandler, 
        IGenericService<TContext> where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly IWrappedAutoMapperConfig _wrapperMapperConfigs;

        /// <summary>
        /// This allows you to access the current DbContext that this instance of the GenericService is using.
        /// That is useful if you need to set up some properties in the DTO that cannot be found in the Entity
        /// For instance, setting up a dropdownlist based on some other database data
        /// </summary>
        public DbContext CurrentContext => _context;

        public GenericService(TContext context, IWrappedAutoMapperConfig wapper)
        {
            _context = context;
            _wrapperMapperConfigs = wapper ?? throw new ArgumentException(nameof(wapper));
        }

        public T ReadSingle<T>(params object[] keys) where T : class
        {    
            T result = null;
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
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
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<T>()} you were looking for.");
            }

            return result;
        }

        public T ReadSingle<T>(Expression<Func<T, bool>> whereExpression) where T : class
        {
            T result;
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
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
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<T>()} you were looking for.");
            }

            return result;
        }

        public void ReadSingleToDto<TDto>(TDto dto, params object[] keys) where TDto : class
        {
            var dtoInfo = typeof(TDto).GetDtoInfoThrowExceptionIfNotThere();
            var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(TDto), dtoInfo.LinkedEntityInfo);
            if (keys.Length == 0)
            {
                //we need to get the keys from the dto
                keys = _context.GetKeysFromDtoInCorrectOrder(dto, dtoInfo.LinkedEntityInfo.EntityType, dtoInfo);
            }
            var result = ((IQueryable<TDto>) projector.Accessor.GetViaKeysWithProject(keys)).SingleOrDefault();
            if (result == null)
            {
                AddError(
                    $"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TDto>()} you were looking for.");
                return;
            }
            //Now copy the result to the original dto
            dtoInfo.ShallowCopyDtoToDto(result, dto);
        }

        public void ReadSingleToDto<TDto>(TDto dto, Expression<Func<TDto, bool>> whereExpression) where TDto : class
        {
            var dtoInfo = typeof(TDto).GetDtoInfoThrowExceptionIfNotThere();
            var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(TDto), dtoInfo.LinkedEntityInfo);
            var result = ((IQueryable<TDto>)projector.Accessor.ProjectAndThenApplyWhereExpression(whereExpression)).SingleOrDefault();
            if (result == null)
            {
                AddError(
                    $"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TDto>()} you were looking for.");
                return;
            }
            //Now copy the result to the original dto
            dtoInfo.ShallowCopyDtoToDto(result, dto);
        }

        public IQueryable<T> ReadManyNoTracked<T>() where T : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            SetMessageIfNotAlreadySet($"Successfully read many {ExtractDisplayHelpers.GetNameForClass<T>()}");
            if (entityInfo.EntityType == typeof(T))
            {
                return _context.Set<T>().AsNoTracking();
            }

            //else its a DTO, so we need to project the entity to the DTO 
            var projector = new CreateMapper(_context, _wrapperMapperConfigs, typeof(T), entityInfo);
            return projector.Accessor.GetManyProjectedNoTracking();
        }

        public T AddNewAndSave<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                _context.Add(entityOrDto);
                _context.SaveChanges();
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
                    CombineStatuses(_context.SaveChangesWithOptionalValidation(dtoInfo.ValidateOnSave));
                    if (IsValid)
                        entity.CopyBackKeysFromEntityToDtoIfPresent(entityOrDto, entityInfo);
                }
            }
            SetMessageIfNotAlreadySet($"Successfully created a {entityInfo.EntityType.GetNameForClass()}");
            return IsValid ? entityOrDto : null;
        }

        public void UpdateAndSave<T>(T entityOrDto, string methodName = null) where T : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(T));
            if (entityInfo.EntityType == typeof(T))
            {
                if (_context.Entry(entityOrDto).State == EntityState.Detached)
                    _context.Update(entityOrDto);
                _context.SaveChanges();
            }
            else
            {
                var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
                var updater = new EntityUpdateHandler<T>(dtoInfo, entityInfo, _wrapperMapperConfigs, _context);
                CombineStatuses(updater.ReadEntityAndUpdateViaDto(entityOrDto, methodName));
                if (IsValid)
                    CombineStatuses(_context.SaveChangesWithOptionalValidation(dtoInfo.ValidateOnSave));        
            }
            SetMessageIfNotAlreadySet($"Successfully updated the {entityInfo.EntityType.GetNameForClass()}");
        }

        public void DeleteAndSave<TEntity>(params object[] keys) where TEntity : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(TEntity));
            if (entityInfo.EntityType != typeof(TEntity))
                throw new NotImplementedException(
                    "You cannot delete a DTO/ViewModel. You must provide a real entity class.");

            var entity = _context.Set<TEntity>().Find(keys);
            if (entity == null)
            {
                AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
                return;
            }
            _context.Remove(entity);
            _context.SaveChanges();
            SetMessageIfNotAlreadySet($"Successfully deleted a {ExtractDisplayHelpers.GetNameForClass<TEntity>()}");
        }

        public void DeleteWithActionAndSave<TEntity>(Func<DbContext, TEntity, IStatusGeneric> runBeforeDelete,
            params object[] keys) where TEntity : class
        {
            var entityInfo = _context.GetUnderlyingEntityInfo(typeof(TEntity));
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
            SetMessageIfNotAlreadySet($"Successfully deleted a {ExtractDisplayHelpers.GetNameForClass<TEntity>()}");
        }

    }
}