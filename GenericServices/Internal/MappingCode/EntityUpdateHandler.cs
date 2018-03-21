// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityUpdateHandler<TDto> : StatusGenericHandler
    {
        private readonly DecodedEntityClass _entityInfo;
        private readonly MapperConfiguration _mapperConfig;
        private readonly IExpandedGlobalConfig _config;

        public EntityUpdateHandler(DecodedEntityClass entityInfo, MapperConfiguration mapperConfig, IExpandedGlobalConfig config)
        {
            _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
            _mapperConfig = mapperConfig ?? throw new ArgumentNullException(nameof(mapperConfig));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void ReadEntityAndUpdateViaDto(TDto dto, string methodName = null)
        {
            //first we need to load it 


            //we look for methods to update a new entity in the following order
            //1. DDD-styled entity: A public access method that fits the DTO
            //2. Normal styled entity: using AutoMapper to update the entity

            if (_entityInfo.CanBeUpdatedViaMethods)
            {

            }
        }
    }
}