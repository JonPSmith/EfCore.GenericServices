// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;
using GenericServices.Internal.LinqBuilders;
using GenericServices.PublicButHidden;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityCreateHandler<TDto> : StatusGenericHandler
        where TDto : class
    {
        private readonly DecodedEntityClass _entityInfo;
        private readonly IWrappedAutoMapperConfig _wrapperMapperConfigs;
        private readonly IExpandedGlobalConfig _config;
        private readonly DecodedDto _dtoInfo;

        public EntityCreateHandler(DecodedEntityClass entityInfo, IWrappedAutoMapperConfig wrapperMapperConfigs, IExpandedGlobalConfig config)
        {
            _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            _wrapperMapperConfigs = wrapperMapperConfigs ?? throw new ArgumentNullException(nameof(wrapperMapperConfigs));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _dtoInfo = typeof(TDto).GetRegisteredDtoInfo() ??
                       throw new NullReferenceException($"The DTO/ViewModel class {typeof(TDto).Name} is not registered as a valid GenericService DTO." +
                                                        " Have you left off the {DecodedDtoExtensions.HumanReadableILinkToEntity} interface?");
        }

        public object CreateEntityAndFillFromDto(TDto dto, string methodCtorName)
        {
            if (!_entityInfo.CanBeCreatedByCtorOrStaticMethod && !_entityInfo.CanBeCreatedViaAutoMapper)
                throw new InvalidOperationException($"I cannot create the entity class {_entityInfo.EntityType.Name} because it has no public constructor, or valid static factory methods.");

            //we look for methods to create/update a new entity in the following order
            //1. A public static method that returns IStatusGeneric (chosing the one with the most parameters that the DTO has too)
            //2. A public parameterised constructor (chosing the one with the most parameters that the DTO has too)
            //3. By creating via parameterless ctor and then using AutoMapper to set the properties

            var decodedName = _dtoInfo.GetSpecifiedName(methodCtorName, CrudTypes.Create);

            if (_entityInfo.CanBeCreatedByCtorOrStaticMethod)
            {
                var ctorStaticToRun = _dtoInfo.GetCtorStaticFactoryToRun(decodedName, _entityInfo);
                var runStatus = BuildCall.RunMethodOrCtorViaLinq(ctorStaticToRun,
                    dto, ctorStaticToRun.PropertiesMatch.MatchedPropertiesInOrder, _config.CurrentContext);
                CombineStatuses(runStatus);
                return runStatus.Result;
            }

            if (_entityInfo.HasPublicParameterlessCtor && _entityInfo.CanBeUpdatedViaProperties)
            {
                var entityInstance = Activator.CreateInstance(_entityInfo.EntityType);
                var mapper = new CreateMapper(_config.CurrentContext, _wrapperMapperConfigs, typeof(TDto), _entityInfo);
                mapper.Accessor.MapDtoToEntity(dto, entityInstance);
                return entityInstance;
            }

            throw new InvalidOperationException(
                $"There was no way to create the entity class {_entityInfo.EntityType.Name} using {decodedName.ToString() ?? "any approach"}.");

        }


    }
}