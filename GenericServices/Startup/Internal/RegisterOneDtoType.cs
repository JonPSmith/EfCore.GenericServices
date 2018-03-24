// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
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
            if (entityType == null)
                throw new InvalidOperationException(
                    $"The DTO/ViewModel class {dtoType.Name} is not registered as a valid GenericService DTO." +
                    $" Have you left off the {DecodedDtoExtensions.HumanReadableILinkToEntity} interface?");
            EntityInfo = entityType.GetRegisteredEntityInfo();

            var assemblyThatDtoIsIn = dtoType.Assembly;
            var configInfo = FindConfigInfoIfPresent(dtoType, entityType, assemblyThatDtoIsIn);
            MapGenerator = new CreateMapGenerator(dtoType, EntityInfo, configuration, configInfo);
            PerDtoConfig = (PerDtoConfig)MapGenerator.Accessor.GetRestOfPerDtoConfig();
        
            var decodeStatus = dtoType.GetOrCreateDtoInfo(EntityInfo, configuration, PerDtoConfig);
            CombineStatuses(decodeStatus);
            DtoInfo = decodeStatus.Result;
            CombineStatuses(DtoInfo);
        }

        private static object FindConfigInfoIfPresent(Type dtoType, Type entityType, Assembly assemblyToScan)
        {
            var perDtoConfigType = dtoType.FormPerDtoConfigType(entityType);
            var types = assemblyToScan.GetTypes().Where(x => x.IsSubclassOf(perDtoConfigType)).ToList();
            if (!types.Any())
                return null;        //no config found
            if (types.Count > 1)
                throw new InvalidOperationException($"I found multiple classes based on PerDtoConfig<{dtoType.Name},{entityType.Name}>, but you are only allowed one."+
                                                    $" They are: {string.Join(", ", types.Select(x => x.Name))}.");
            return Activator.CreateInstance(types.First());
        }
    }
}