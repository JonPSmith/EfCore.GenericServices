// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericLibsBase;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;

namespace GenericServices.Extensions.Internal
{
    internal class RegisterOneDtoType : StatusGenericHandler
    {
        public DecodedDto DtoInfo { get; }

        public PerDtoConfig PerDtoConfig { get; }

        public CreateMapGenerator MapGenerator { get; }

        public RegisterOneDtoType(Type dtoType, IGenericServiceConfig configuration)
        {
            var entityType = dtoType.GetLinkedEntityFromDto();
            var entityInfo = entityType.GetRegisteredEntityInfo();
            if (entityInfo == null)
                AddError(
                    $"The entity {entityType} found in the  {DecodedDtoExtensions.HumanReadableILinkToEntity} interface of the DTO/VM {dtoType.Name} " +
                    "cannot be found. The class must be one that EF Core maps to the database.");

            var configInfo = CreateConfigInfoIfPresent(dtoType);
            MapGenerator = new CreateMapGenerator(dtoType, entityInfo, configuration, configInfo);
            PerDtoConfig = (PerDtoConfig)MapGenerator.Accessor.GetRestOfPerDtoConfig();
        
            var decodeStatus = dtoType.GetOrCreateDtoInfo(entityInfo, configuration, PerDtoConfig);
            CombineErrors(decodeStatus);
            DtoInfo = decodeStatus.Result;
        }

        private object CreateConfigInfoIfPresent(Type dtoType)
        {
            var configType = dtoType.GetConfigTypeFromDto();
            return configType == null
                ? null
                : Activator.CreateInstance(configType);
        }
    }
}