// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper.QueryableExtensions;
using GenericServices.Configuration.Internal;
using GenericServices.ExtensionMethods;
using GenericServices.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.LinqBuilders;
using GenericServices.Internal.MappingCode;
using GenericServices.Setup;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.PublicButHidden
{
	/// <summary>
	/// This is the sync version of GenericServices' CRUD, which assumes you have one DbContext which the CrudServices setup code will register to the DbContext type
	/// You should use this with dependency injection to get an instance of the sync CrudServices
	/// </summary>
	public class CrudServices : CrudServices<DbContext>, ICrudServices
	{
		/// <summary>
		/// CrudServices needs the correct DbContext and the AutoMapper config
		/// </summary>
		/// <param name="context"></param>
		/// <param name="configAndMapper"></param>
		/// <param name="createNewDBContext"></param>
		public CrudServices(DbContext context, IWrappedConfigAndMapper configAndMapper, IDbContextService createNewDBContext) : base(context, configAndMapper, createNewDBContext)
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
		private readonly IWrappedConfigAndMapper _configAndMapper;
		private readonly IDbContextService _createNewDBContext;

		/// <inheritdoc />
		public DbContext Context => _context;

		/// <summary>
		/// CrudServices needs the correct DbContext and the AutoMapper config
		/// </summary>
		/// <param name="context"></param>
		/// <param name="configAndMapper"></param>
		/// <param name="createNewDBContext"></param>
		public CrudServices(TContext context, IWrappedConfigAndMapper configAndMapper, IDbContextService createNewDBContext)
		{
			_context = context;
			_configAndMapper = configAndMapper ?? throw new ArgumentException(nameof(configAndMapper));
			_createNewDBContext = createNewDBContext;
		}

		/// <inheritdoc />
		public T ReadSingle<T>(params object[] keys) where T : class
		{
			T result;
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
			if (entityInfo.EntityStyle == EntityStyles.DbQuery)
				throw new InvalidOperationException($"The class {entityInfo.EntityType.Name} of style {entityInfo.EntityStyle} cannot be used in a Find.");
			if (entityInfo.EntityType == typeof(T))
			{
				result = _context.Set<T>().Find(keys);
			}
			else
			{
				//else its a DTO, so we need to project the entity to the DTO and select the single element
				var projector = new CreateMapper(_context, _configAndMapper, typeof(T), entityInfo);
				result = ((IQueryable<T>)projector.Accessor.GetViaKeysWithProject(keys)).SingleOrDefault();
			}

			if (result != null) return result;

			if (_configAndMapper.Config.NoErrorOnReadSingleNull)
				Message = $"The {entityInfo.EntityType.GetNameForClass()} was not found.";
			else
				AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");

			return null;
		}

		/// <inheritdoc />
		public T ReadSingle<T>(Expression<Func<T, bool>> whereExpression) where T : class
		{
			T result;
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
			if (entityInfo.EntityType == typeof(T))
			{
				result = entityInfo.GetReadableEntity<T>(_context).Where(whereExpression).SingleOrDefault();
			}
			else
			{
				//else its a DTO, so we need to project the entity to the DTO and select the single element
				var projector = new CreateMapper(_context, _configAndMapper, typeof(T), entityInfo);
				result = ((IQueryable<T>)projector.Accessor.ProjectAndThenApplyWhereExpression(whereExpression)).SingleOrDefault();
			}

			if (result != null) return result;

			if (_configAndMapper.Config.NoErrorOnReadSingleNull)
				Message = $"The {entityInfo.EntityType.GetNameForClass()} was not found.";
			else
				AddError($"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were looking for.");

			return null;
		}

		/// <inheritdoc />
		public IQueryable<T> ReadManyNoTracked<T>() where T : class
		{
			Message = $"Successfully read many {ExtractDisplayHelpers.GetNameForClass<T>()}";
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
			if (entityInfo.EntityType == typeof(T))
			{
				return entityInfo.GetReadableEntity<T>(_context).AsNoTracking();
			}

			//else its a DTO, so we need to project the entity to the DTO
			var projector = new CreateMapper(_context, _configAndMapper, typeof(T), entityInfo);
			return projector.Accessor.GetManyProjectedNoTracking();
		}

		/// <inheritdoc />
		public IQueryable<TDto> ProjectFromEntityToDto<TEntity, TDto>(Func<IQueryable<TEntity>, IQueryable<TEntity>> query) where TEntity : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
			return query(entityInfo.GetReadableEntity<TEntity>(_context))
				.ProjectTo<TDto>(_configAndMapper.MapperReadConfig);
		}

		/// <inheritdoc />
		public T CreateAndSave<T>(T entityOrDto, string ctorOrStaticMethodName = null) where T : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
			entityInfo.CheckCanDoOperation(CrudTypes.Create);
			Message = $"Successfully created a {entityInfo.EntityType.GetNameForClass()}";
			if (entityInfo.EntityType == typeof(T))
			{
				_context.Add(entityOrDto);
				CombineStatuses(_context.SaveChangesWithOptionalValidation(
					_configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config));
			}
			else
			{
				using (DbContext context = _createNewDBContext.CreateNew())
				{
					var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();

					var creator = new EntityCreateHandler<T>(dtoInfo, entityInfo, _configAndMapper, context);
					var entity = creator.CreateEntityAndFillFromDto(entityOrDto, ctorOrStaticMethodName);
					CombineStatuses(creator);
					if (IsValid)
					{
						_context.Add(entity);
						CombineStatuses(_context.SaveChangesWithOptionalValidation(dtoInfo.ShouldValidateOnSave(_configAndMapper.Config), _configAndMapper.Config));
						if (IsValid)
							entity.CopyBackKeysFromEntityToDtoIfPresent(entityOrDto, entityInfo);
					}
				}
			}
			return IsValid ? entityOrDto : null;
		}

		/// <inheritdoc />
		public void UpdateAndSave<T>(T entityOrDto, params Expression<Func<T, object>>[] includes) where T : class
		{
			UpdateAndSave(entityOrDto, methodName: null, includes: includes);
		}

		/// <inheritdoc />
		public void UpdateAndSave<T>(T entityOrDto, string methodName, params Expression<Func<T, object>>[] includes) where T : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(T));
			entityInfo.CheckCanDoOperation(CrudTypes.Update);
			Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";

			CheckIncludes(entityOrDto, includes?.Select(x => x.ToFullMemberNameString()).ToList(), new List<Type>());

			if (entityInfo.EntityType == typeof(T))
			{
				if (!_context.Entry(entityOrDto).IsKeySet)
					throw new InvalidOperationException($"The primary key was not set on the entity class {typeof(T).Name}. For an update we expect the key(s) to be set (otherwise it does a create).");
				if (_context.Entry(entityOrDto).State == EntityState.Detached)
					_context.Update(entityOrDto);
				CombineStatuses(_context.SaveChangesWithOptionalValidation(
					_configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config));
			}
			else
			{
				var dtoInfo = typeof(T).GetDtoInfoThrowExceptionIfNotThere();
				var updater = new EntityUpdateHandler<T>(dtoInfo, entityInfo, _configAndMapper, _context, _createNewDBContext);
				CombineStatuses(updater.ReadEntityAndUpdateViaDto(entityOrDto, methodName, includes));
				if (IsValid)
					CombineStatuses(_context.SaveChangesWithOptionalValidation(
						dtoInfo.ShouldValidateOnSave(_configAndMapper.Config), _configAndMapper.Config));
			}
		}

		internal void CheckIncludes<T>(T entityOrDto, List<string> includesStrings, List<Type> alreadyUsedObjects)
		{
			if (entityOrDto == null)
				return;

			alreadyUsedObjects.Add(typeof(T));

			// NTW
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
			{
				Type entityType = propertyInfo.PropertyType;

				if (entityType.Name.Equals(typeof(ICollection<>).Name) && entityType.IsGenericType)
				{
					entityType = entityType.GetGenericArguments().First();
				}

				Type interfaceType = entityType.GetInterface(typeof(ILinkToEntity<>).Name);
				if (interfaceType != null)
				{
					entityType = interfaceType.GenericTypeArguments.FirstOrDefault() ?? entityType;
				}

				if (this.Context.Model.FindEntityType(entityType) != null && (includesStrings == null || !includesStrings.Contains(propertyInfo.Name)) && !alreadyUsedObjects.Contains(entityType))
					propertyInfo.SetValue(entityOrDto, null);
				else if (this.Context.Model.FindEntityType(entityType) != null && !alreadyUsedObjects.Contains(entityType))
				{
					List<object> args = new List<object>();
					args.Add(propertyInfo.GetValue(entityOrDto));
					args.Add(includesStrings.Where(x => x.StartsWith(propertyInfo.Name)).Select(x => String.Join(".", x.Split('.').Skip(1))).Where(x => !String.IsNullOrEmpty(x)).ToList());
					args.Add(alreadyUsedObjects);

					this.GetType().GetMethod("CheckIncludes", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(propertyInfo.PropertyType).Invoke(this, args.ToArray());
				}
			}
		}

		/// <inheritdoc />
		public void UpdateWithActionAndSave<TDto, TEntity>(Func<DbContext, TEntity, IStatusGeneric> runBeforeUpdate, TDto entityOrDto, string methodName = null, params Expression<Func<TDto, object>>[] includes) where TDto : class where TEntity : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TDto));

			object[] idPropertyValues = new object[entityInfo.PrimaryKeyProperties.Count];

			int idx = -1;

			entityInfo.PrimaryKeyProperties.ForEach(kP => idPropertyValues[++idx] = entityOrDto.GetType().GetProperty(kP.Name).GetValue(entityOrDto));

			var entity = _context.Set<TEntity>().Find(idPropertyValues);

			CombineStatuses(runBeforeUpdate(_context, entity));
			if (!IsValid) return;

			Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";
			if (entityInfo.EntityType == typeof(TDto))
			{
				if (!_context.Entry(entityOrDto).IsKeySet)
					throw new InvalidOperationException($"The primary key was not set on the entity class {typeof(TDto).Name}. For an update we expect the key(s) to be set (otherwise it does a create).");
				if (_context.Entry(entityOrDto).State == EntityState.Detached)
					_context.Update(entityOrDto);
				CombineStatuses(_context.SaveChangesWithOptionalValidation(
					_configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config));
			}
			else
			{
				var dtoInfo = typeof(TDto).GetDtoInfoThrowExceptionIfNotThere();
				var updater = new EntityUpdateHandler<TDto>(dtoInfo, entityInfo, _configAndMapper, _context, _createNewDBContext);
				CombineStatuses(updater.ReadEntityAndUpdateViaDto(entityOrDto, methodName, includes));
				if (IsValid)
					CombineStatuses(_context.SaveChangesWithOptionalValidation(
						dtoInfo.ShouldValidateOnSave(_configAndMapper.Config), _configAndMapper.Config));
			}
		}

		/// <inheritdoc />
		public TEntity UpdateAndSave<TEntity>(JsonPatchDocument<TEntity> patch, params object[] keys) where TEntity : class
		{
			return LocalUpdateAndSave(patch, () => _context.Find<TEntity>(keys));
		}

		/// <inheritdoc />
		public TEntity UpdateAndSave<TEntity>(JsonPatchDocument<TEntity> patch, Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
		{
			return LocalUpdateAndSave(patch, () => _context.Set<TEntity>().SingleOrDefault(whereExpression));
		}

		/// <summary>
		/// Local version of UpdateAndSave with JsonPatch - contains the common code
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="patch"></param>
		/// <param name="getEntity"></param>
		/// <returns></returns>
		private TEntity LocalUpdateAndSave<TEntity>(JsonPatchDocument<TEntity> patch, Func<TEntity> getEntity)
			where TEntity : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
			entityInfo.CheckCanDoOperation(CrudTypes.Update);
			Message = $"Successfully updated the {entityInfo.EntityType.GetNameForClass()}";
			if (entityInfo.EntityType != typeof(TEntity))
				throw new NotImplementedException(
					$"I could not find the entity class {typeof(TEntity).Name}. JsonPatch only works on entity classes.");

			var entity = getEntity();
			if (entity != null)
				patch.ApplyTo(entity, error => AddError(error.ErrorMessage));
			else
				AddError(
					$"Sorry, I could not find the {entityInfo.EntityType.GetNameForClass()} you were trying to update.");
			if (IsValid)
				CombineStatuses(_context.SaveChangesWithOptionalValidation(
					_configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config));

			return entity;
		}

		/// <inheritdoc />
		public void DeleteAndSave<TEntity>(params object[] keys) where TEntity : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
			entityInfo.CheckCanDoOperation(CrudTypes.Delete);
			Message = $"Successfully deleted a {entityInfo.EntityType.GetNameForClass()}";
			if (entityInfo.EntityType != typeof(TEntity))
				throw new NotImplementedException(
					"You cannot delete a DTO/ViewModel. You must provide a real entity class.");

			var whereWithKeys = entityInfo.PrimaryKeyProperties.CreateFilter<TEntity>(keys);
			var entity = _context.Set<TEntity>().SingleOrDefault(whereWithKeys);
			if (entity == null)
			{
				AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
			}
			if (!IsValid) return;

			_context.Remove(entity);
			CombineStatuses(_context.SaveChangesWithOptionalValidation(
				_configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config));
		}

		/// <inheritdoc />
		public void DeleteWithActionAndSave<TEntity>(Func<DbContext, TEntity, IStatusGeneric> runBeforeDelete,
			params object[] keys) where TEntity : class
		{
			var entityInfo = _context.GetEntityInfoThrowExceptionIfNotThere(typeof(TEntity));
			entityInfo.CheckCanDoOperation(CrudTypes.Delete);
			Message = $"Successfully deleted a {entityInfo.EntityType.GetNameForClass()}";
			if (entityInfo.EntityType != typeof(TEntity))
				throw new NotImplementedException(
					"You cannot delete a DTO/ViewModel. You must provide a real entity class.");

			var whereWithKeys = entityInfo.PrimaryKeyProperties.CreateFilter<TEntity>(keys);
			var entity = _context.Set<TEntity>().IgnoreQueryFilters().SingleOrDefault(whereWithKeys);
			if (entity == null)
			{
				AddError($"Sorry, I could not find the {ExtractDisplayHelpers.GetNameForClass<TEntity>()} you wanted to delete.");
				return;
			}

			CombineStatuses(runBeforeDelete(_context, entity));
			if (!IsValid) return;

			_context.Remove(entity);
			CombineStatuses(_context.SaveChangesWithOptionalValidation(
				_configAndMapper.Config.DirectAccessValidateOnSave, _configAndMapper.Config));
		}
	}
}