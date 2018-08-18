// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using GenericServices.Internal.LinqBuilders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityUpdateHandler<TDto> : StatusGenericHandler
        where TDto : class
    {
        private readonly DecodedDto _dtoInfo;
        private readonly DecodedEntityClass _entityInfo;
        private readonly IWrappedConfigAndMapper _configAndMapper;
        private readonly DbContext _context;

        public EntityUpdateHandler(DecodedDto dtoInfo, DecodedEntityClass entityInfo, IWrappedConfigAndMapper configAndMapper, DbContext context)
        {
            _dtoInfo = dtoInfo ?? throw new ArgumentNullException(nameof(dtoInfo));
            _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            _configAndMapper = configAndMapper ?? throw new ArgumentNullException(nameof(configAndMapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IStatusGeneric ReadEntityAndUpdateViaDto(TDto dto, string methodName)
        {
            //first we need to load it 
            var keys = _context.GetKeysFromDtoInCorrectOrder(dto, _dtoInfo);
            var mapper = new CreateMapper(_context, _configAndMapper, typeof(TDto), _entityInfo);
            var entity = mapper.Accessor.ReturnExistingEntity(keys);
            if (entity == null)
                return new StatusGenericHandler()
                    .AddError(
                        $"Sorry, I could not find the {_entityInfo.EntityType.GetNameForClass()} you were trying to update.");

            //we look for methods to update a new entity in the following order
            //1. DDD-styled entity: A public access method that fits the DTO
            //2. Standard styled entity: using AutoMapper to update the entity

            var decodedName = _dtoInfo.GetSpecifiedName(methodName, CrudTypes.Update);

            if (_entityInfo.CanBeUpdatedViaMethods && decodedName.NameType != DecodedNameTypes.AutoMapper)
            {
                //1. DDD-styled entity: A public access method that fits the DTO

                //This gets one method to run. If it can't be found, or there are too many matches it throws an exception
                var methodToUse = _dtoInfo.GetMethodToRun(decodedName, _entityInfo);

                //This runs the method via LINQ
                return BuildCall.RunMethodViaLinq(methodToUse.Method, 
                    dto, entity, methodToUse.PropertiesMatch.MatchedPropertiesInOrder.ToList(), _context);
            }

            if (_entityInfo.CanBeUpdatedViaProperties)
            {
                //2. Standard styled entity: using AutoMapper to update the entity
                mapper.Accessor.MapDtoToEntity(dto, entity);
                return new StatusGenericHandler();
            }

            throw new InvalidOperationException(
                $"There was no way to update the entity class {_entityInfo.EntityType.Name} using {decodedName.ToString() ?? "any approach"}.");
        }
    }
}