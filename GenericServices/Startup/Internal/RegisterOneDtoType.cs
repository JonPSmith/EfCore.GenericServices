// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.Internal.MappingCode;

namespace GenericServices.Startup.Internal
{
    internal class RegisterOneDtoType : StatusGenericHandler
    {
        public DecodedEntityClass EntityInfo { get; }
        public DecodedDto DtoInfo { get; }

        public PerDtoConfig PerDtoConfig { get; }

        public CreateMapGenerator MapGenerator { get; }

        public RegisterOneDtoType(Type dtoType, IExpandedGlobalConfig configuration)
        {
            Header = dtoType.Name;
            var entityType = dtoType.GetLinkedEntityFromDto();
            EntityInfo = entityType.GetRegisteredEntityInfo();
            if (EntityInfo == null)
                AddError(
                    $"The entity {entityType} found in the  {DecodedDtoExtensions.HumanReadableILinkToEntity} interface of the DTO/VM {dtoType.Name} " +
                    "cannot be found. The class must be one that EF Core maps to the database.");

            var configInfo = CreateConfigInfoIfPresent(dtoType);
            MapGenerator = new CreateMapGenerator(dtoType, EntityInfo, configuration, configInfo);
            PerDtoConfig = (PerDtoConfig)MapGenerator.Accessor.GetRestOfPerDtoConfig();
        
            var decodeStatus = dtoType.GetOrCreateDtoInfo(EntityInfo, configuration, PerDtoConfig);
            CombineStatuses(decodeStatus);
            DtoInfo = decodeStatus.Result;
            CombineStatuses(DtoInfo);
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