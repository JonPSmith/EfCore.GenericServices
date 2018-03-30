// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup.Internal
{
    internal class SetupAllDtosAndMappings : StatusGenericHandler, IGenericServicesSetupPart2
    {
        private readonly IGenericServicesConfig _publicConfig;
        private Type[] _contextTypes;

        MappingProfile _readProfile = new MappingProfile(false);
        MappingProfile _saveProfile = new MappingProfile(true);

        public IWrappedAutoMapperConfig AutoMapperConfig { get;}
        public IServiceCollection Services { get; }

        public SetupAllDtosAndMappings(IGenericServicesSetupPart1 part1Data, Assembly[] assembliesToScan)
        {
            _publicConfig = part1Data?.PublicConfig ??
                           throw new NullReferenceException($"{nameof(part1Data)}.{nameof(IGenericServicesSetupPart1.PublicConfig)}");
            _contextTypes = part1Data?.ContextTypes ??
                           throw new NullReferenceException($"{nameof(part1Data)}.{nameof(IGenericServicesSetupPart1.ContextTypes)}");
            Services = part1Data?.Services ??
                       throw new NullReferenceException($"{nameof(part1Data)}.{nameof(Services)}");

            //Add ForAllPropertyMaps to start the Mapping
            foreach (var assembly in assembliesToScan)
            {
                RegisterDtosInAssemblyAndBuildMaps(assembly);
            }

            if (IsValid)
                //If errors then don't set up the mappings
                return;

            AutoMapperConfig = CreateWrappedAutoMapperConfig(_readProfile, _saveProfile);
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(_readProfile);
                cfg.AddProfile(_saveProfile);
            });
        }

        public static IWrappedAutoMapperConfig CreateWrappedAutoMapperConfig(MappingProfile readProfile, MappingProfile saveProfile)
        {
            var mapperReadConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            });
            var mapperSaveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            });
            return new WrappedAutoMapperConfig(mapperReadConfig, mapperSaveConfig);
        }

        private void RegisterDtosInAssemblyAndBuildMaps(Assembly assemblyToScan)
        {
            Header = $"Scanning {assemblyToScan.GetName()}";
            var allTypesInAssembly = assemblyToScan.GetTypes();
            var allLinkToEntityClasses = allTypesInAssembly
                .Where(x => x.GetLinkedEntityFromDto() != null);

            foreach (var dtoType in allLinkToEntityClasses)
            {
                var dtoRegister = new RegisterOneDtoType(dtoType, allTypesInAssembly, _publicConfig);
                if (!dtoRegister.IsValid)
                {
                    CombineStatuses(dtoRegister);
                    continue;
                }

                //Now build the mapping using the ConfigGenerator in the register
                dtoRegister.ConfigGenerator.Accessor.AddReadMappingToProfile(_readProfile);
                //Only add a mapping if AutoMapper can be used to update/create the entity
                if (dtoRegister.EntityInfo.EntityStyle != EntityStyles.DDDStyled &&
                    dtoRegister.EntityInfo.EntityStyle != EntityStyles.ReadOnly)
                {
                    dtoRegister.ConfigGenerator.Accessor.AddSaveMappingToProfile(_saveProfile);
                }
            }
        }



    }
}