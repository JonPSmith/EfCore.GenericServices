// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using AutoMapper;
using GenericLibsBase;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.MappingCode
{
    internal class EntityCreateHandler<TDto> where TDto : class
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

        public IStatusGeneric CreateEntityAndFillFromDto(TDto dto)
        {
            var status = new StatusGenericHandler();
            if (!_entityInfo.CanBeCreated)
                throw new InvalidOperationException($"I cannot create the entity class {_entityInfo.EntityType.Name} because it has no public constructor, or valid static factory methods.");

            //we look for methods to create/update a new entity in the following order
            //1. A public static method that returns IStatusGeneric (chosing the one with the most parameters that the DTO has too)
            //2. A public parameterised constructor (chosing the one with the most parameters that the DTO has too)
            //3. By creating via parameterless ctor and then using AutoMapper to set the properties

            var dtoInfo = typeof(TDto).GetDtoInfo();
            var bestMatch = BestMethodCtorMatch.FindMatch(dtoInfo.PropertyInfo.Select(x => x.PropertyInfo).ToImmutableList(), 
                _entityInfo.PublicStaticFactoryMethods);
            if (bestMatch == null || bestMatch.Score < 1)
            {
                var bestCtorMatch = BestMethodCtorMatch.FindMatch(dtoInfo.PropertyInfo.Select(x => x.PropertyInfo).ToImmutableList(),
                    _entityInfo.PublicCtors);
                if (bestCtorMatch != null && bestCtorMatch.Score > bestMatch.Score)
                    bestMatch = bestCtorMatch;
            }

            if (bestMatch?.Score >= 0.999999)
            {
                //Build call to the method/ctor
            }
            else if (_entityInfo.HasPublicParameterlessCtor && _entityInfo.CanBeUpdatedViaProperties)
            {
                //set up AutoMapper mappings
            }


            return status;

        }


    }
}