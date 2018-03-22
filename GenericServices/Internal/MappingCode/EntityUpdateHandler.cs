// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using GenericServices.Internal.LinqBuilders;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityUpdateHandler<TDto> : StatusGenericHandler
        where TDto : class
    {
        private readonly DecodedEntityClass _entityInfo;
        private readonly IWrappedAutoMapperConfig _wrapperMapperConfigs;
        private readonly IExpandedGlobalConfig _config;
        private readonly DecodedDto _dtoInfo;

        public EntityUpdateHandler(DecodedEntityClass entityInfo, IWrappedAutoMapperConfig wapper, IExpandedGlobalConfig config)
        {
            _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            _wrapperMapperConfigs = wapper ?? throw new ArgumentNullException(nameof(wapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dtoInfo = typeof(TDto).GetRegisteredDtoInfo() ??
                       throw new NullReferenceException($"The DTO/ViewModel class {typeof(TDto).Name} is not registered as a valid GenericService DTO."+
                                                        " Have you left off the {DecodedDtoExtensions.HumanReadableILinkToEntity} interface?");
        }

        public IStatusGeneric ReadEntityAndUpdateViaDto(TDto dto, string methodName)
        {
            //first we need to load it 
            var keys = _config.CurrentContext.GetKeysFromDtoInCorrectOrder(dto, _entityInfo.EntityType, _dtoInfo);
            var mapper = new CreateMapper(_config.CurrentContext, _wrapperMapperConfigs, typeof(TDto), _entityInfo);
            var entity = mapper.Accessor.ReturnExistingEntity(keys);

            //we look for methods to update a new entity in the following order
            //1. DDD-styled entity: A public access method that fits the DTO
            //2. Normal styled entity: using AutoMapper to update the entity

            var decodedName = _dtoInfo.GetSpecifiedName(methodName, CrudTypes.Update);

            if (_entityInfo.CanBeUpdatedViaMethods && decodedName.NameType != DecodedNameTypes.AutoMapper)
            {
                //1. DDD-styled entity: A public access method that fits the DTO

                //This gets one method to run. If it can't be found, or there are too many matches it throws an exception
                var methodToUse = _dtoInfo.GetMethodToRun(decodedName, _entityInfo);

                //This runs the method via LINQ
                return BuildCall.RunMethodViaLinq(methodToUse.Method, 
                    dto, entity, methodToUse.PropertiesMatch.MatchedPropertiesInOrder, _config.CurrentContext);
            }

            if (_entityInfo.CanBeUpdatedViaProperties && _entityInfo.HasPublicParameterlessCtor)
            {
                //2. Normal styled entity: using AutoMapper to update the entity
                mapper.Accessor.MapDtoToEntity(dto, entity);
                return new StatusGenericHandler();
            }

            throw new InvalidOperationException(
                $"There was no way to update the entity class {_entityInfo.EntityType.Name} using {decodedName.ToString() ?? "any approach"}.");
        }
    }
}