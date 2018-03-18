// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using GenericLibsBase;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Extensions.Internal
{
    internal class SetupAllDtos : StatusGenericHandler
    {
        private IGenericServiceConfig _configuration;

        WrappedAutoMapperConfig AutoMapperConfig { get; set; }

        public SetupAllDtos(IGenericServiceConfig configuration, Assembly[] assembliesToScan)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var allMaps = new List<IMappingExpression>();
            foreach (var assembly in assembliesToScan)
            {
                
            }
        }


        public List<IMappingExpression> RegisterDtosInAssemblyAndBuildMap(Assembly assemblyToScan)
        {

            var mapsThisassembly = new List<IMappingExpression>();
            var allTypesInAssembly = assemblyToScan.GetTypes();
            var allLinkToEntityClasses = allTypesInAssembly
                .Where(x => x.GetLinkedEntityFromDto() != null);

            foreach (var dtoType in allLinkToEntityClasses)
            {
                var entityType = dtoType.GetLinkedEntityFromDto();
                var entityInfo = entityType.GetRegisteredEntityInfo();
                if (entityInfo == null)
                    AddError(
                        $"The entity {entityType} found in the  {DecodedDtoExtensions.HumanReadableILinkToEntity} interface of the DTO/VM {dtoType.Name} "+
                        "cannot be found. The class must be one that EF Core maps to the database.");

                var configInfo = CreateConfigInfoIfPresent(dtoType);

                var decodeStatus = dtoType.GetOrCreateDtoInfo(entityInfo);
                if (decodeStatus.HasErrors)
                {
                    CombineErrors(decodeStatus);
                    continue;
                }
                mapsThisassembly.AddRange(CreateAutoMapperMappings(decodeStatus.Result, configInfo));
            }
            throw new NotImplementedException();
        }

        private IEnumerable<IMappingExpression> CreateAutoMapperMappings(DecodedDto decodedDto, object configInfo)
        {
            throw new NotImplementedException();
        }

        private object CreateConfigInfoIfPresent(Type dtoType)
        {
            var configType = dtoType.GetConfigFromDto();
            return configType == null 
                ? null 
                : Activator.CreateInstance(configType);
        }
    }
}