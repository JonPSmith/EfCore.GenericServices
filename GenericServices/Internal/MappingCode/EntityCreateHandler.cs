// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericLibsBase;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityCreateHandler
    {
        private readonly DbContext _context;
        private readonly IMapper _mapper;
        private readonly DecodedEntityClass _entityInfo;

        public EntityCreateHandler(DbContext context, IMapper mapper, DecodedEntityClass entityInfo)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _entityInfo = entityInfo ?? throw new ArgumentNullException(nameof(entityInfo));
        }

        public IStatusGeneric CreateEntityAndFillFromDto(object dto)
        {
            var status = new StatusGenericHandler();
            if (!_entityInfo.CanBeCreated)
                throw new InvalidOperationException($"I cannot create the entity class {_entityInfo.EntityType.Name} because it has no public constructor, or valid static factory methods.");

            //we look for methods to create/update a new entity in the following order
            //1. A public static method that returns IStatusGeneric (chosing the one with the most parameters that the DTO has too)
            //2. A public parameterised constructor (chosing the one with the most parameters that the DTO has too)
            //3. By creating via parameterless ctor and then using AutoMapper to set the properties

            //var dtoPropertiesToConsider = 
            //var chosenStaticMethod = FindBestParameterMatch( _entityInfo.EntityClassInfo.PublicStaticFactoryMethods);

            throw new NotImplementedException();
        }
    }
}