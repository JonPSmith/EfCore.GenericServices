// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.LinqBuilders;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityCreateHandler<TDto> : StatusGenericHandler
        where TDto : class
    {
        private readonly DecodedDto _dtoInfo;
        private readonly DecodedEntityClass _entityInfo;
        private readonly IWrappedConfigAndMapper _configAndMapper;
        private readonly DbContext _context;

        public EntityCreateHandler(DecodedDto dtoInfo, DecodedEntityClass entityInfo, IWrappedConfigAndMapper configAndMapper, DbContext context)
        {
            _dtoInfo = dtoInfo ?? throw new ArgumentNullException(nameof(dtoInfo));
            _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            _configAndMapper = configAndMapper ?? throw new ArgumentNullException(nameof(configAndMapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public object CreateEntityAndFillFromDto(TDto dto, string methodCtorName)
        {
            if (!_entityInfo.CanBeCreatedByCtorOrStaticMethod && !_entityInfo.CanBeCreatedViaAutoMapper)
                throw new InvalidOperationException($"I cannot create the entity class {_entityInfo.EntityType.Name} because it has no public constructor, or valid static creator methods.");

            //we look for methods to create/update a new entity in the following order
            //1. A public static method that returns IStatusGeneric (chosing the one with the most parameters that the DTO has too)
            //2. A public parameterised constructor (chosing the one with the most parameters that the DTO has too)
            //3. By creating via parameterless ctor and then using AutoMapper to set the properties

            var decodedName = _dtoInfo.GetSpecifiedName(methodCtorName, CrudTypes.Create);

            if (_entityInfo.CanBeCreatedByCtorOrStaticMethod)
            {
                var ctorStaticToRun = _dtoInfo.GetCtorStaticCreatorToRun(decodedName, _entityInfo);
                var runStatus = BuildCall.RunMethodOrCtorViaLinq(ctorStaticToRun,
                    dto, ctorStaticToRun.PropertiesMatch.MatchedPropertiesInOrder.ToList(), _context);
                CombineStatuses(runStatus);
                return runStatus.Result;
            }

            if (_entityInfo.HasPublicParameterlessCtor && _entityInfo.CanBeUpdatedViaProperties)
            {
                var entityInstance = Activator.CreateInstance(_entityInfo.EntityType);
                var mapper = new CreateMapper(_context, _configAndMapper, typeof(TDto), _entityInfo);
                mapper.Accessor.MapDtoToEntity(dto, entityInstance);
                return entityInstance;
            }

            throw new InvalidOperationException(
                $"There was no way to create the entity class {_entityInfo.EntityType.Name} using {decodedName.ToString() ?? "any approach"}.");

        }


    }
}