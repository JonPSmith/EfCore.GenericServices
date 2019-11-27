// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.Setup.Internal;
using StatusGeneric;

namespace GenericServices.Setup.Internal
{
    internal class RegisterOneDtoType : StatusGenericHandler
    {
        public DecodedEntityClass EntityInfo { get; }
        public DecodedDto DtoInfo { get; }

        public PerDtoConfig PerDtoConfig { get; }

        public CreateConfigGenerator ConfigGenerator { get; }

        public RegisterOneDtoType(Type dtoType, Type[] typesInAssembly, IGenericServicesConfig configuration)
        {

            Header = dtoType.Name;
            var entityType = dtoType.GetLinkedEntityFromDto(err => AddError(err));
            if (!IsValid)
                return;
            if (entityType == null)
            {
                AddError($"The class {dtoType.Name} is not registered as a valid CrudServices DTO/ViewModel." +
                         $" Have you left off the {DecodedDtoExtensions.HumanReadableILinkToEntity} interface?");
                return;
            }

            EntityInfo = entityType.GetRegisteredEntityInfo();
            if (EntityInfo == null)
            {
                AddError($"The DTO/ViewModel class {dtoType.Name} is linked to a entity class {entityType.Name}," +
                         $" but I couldn't find that class in the application's DbContext(s) you gave me to scan");
                return;
            }

            var perDtoConfig = FindConfigInfoIfPresent(dtoType, entityType, typesInAssembly);
            if (!IsValid)
                return;
            ConfigGenerator = new CreateConfigGenerator(dtoType, EntityInfo, perDtoConfig);
            PerDtoConfig = perDtoConfig as PerDtoConfig;            
            var decodeStatus = dtoType.GetOrCreateDtoInfo(EntityInfo, configuration, PerDtoConfig);
            CombineStatuses(decodeStatus);
            DtoInfo = decodeStatus.Result;
        }

        private object FindConfigInfoIfPresent(Type dtoType, Type entityType, Type[] typesToScan)
        {
            var perDtoConfigType = dtoType.FormPerDtoConfigType(entityType);
            var types = typesToScan.Where(x => x.IsSubclassOf(perDtoConfigType)).ToList();
            if (!types.Any())
                return null;        //no config found
            if (types.Count > 1)
            {
                AddError($"I found multiple classes based on PerDtoConfig<{dtoType.Name},{entityType.Name}>, but you are only allowed one."+
                         $" They are: {string.Join(", ", types.Select(x => x.Name))}.");
                return null;
            }
            return Activator.CreateInstance(types.First());
        }
    }
}