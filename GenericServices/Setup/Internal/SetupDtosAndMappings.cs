// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using StatusGeneric;

namespace GenericServices.Setup.Internal
{
    internal class SetupDtosAndMappings : StatusGenericHandler
    {
        readonly MappingProfile _readProfile = new MappingProfile(false);
        readonly MappingProfile _saveProfile = new MappingProfile(true);

        public IGenericServicesConfig PublicConfig { get; }

        public SetupDtosAndMappings(IGenericServicesConfig publicConfig)
        {
            PublicConfig = publicConfig ?? throw new ArgumentNullException(nameof(publicConfig));
        }

        public IWrappedConfigAndMapper ScanAllAssemblies(Assembly[] assembliesToScan, IGenericServicesConfig config)
        {
            if (assembliesToScan == null || assembliesToScan.Length == 0)
                throw new ArgumentException("There were no assembles to scan!", nameof(assembliesToScan));
            foreach (var assembly in assembliesToScan)
            {
                RegisterDtosInAssemblyAndBuildMaps(assembly);
            }

            if (!IsValid)
                //If errors then don't set up the mappings
                return null;

            return CreateConfigAndMapper(config, _readProfile, _saveProfile);
        }

        public static IWrappedConfigAndMapper CreateConfigAndMapper(IGenericServicesConfig config, MappingProfile readProfile, MappingProfile saveProfile)
        {
            var mapperReadConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(readProfile);
            });
            var mapperSaveConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(saveProfile);
            });
            return new WrappedAndMapper(config, mapperReadConfig, mapperSaveConfig);
        }

        public static void SetupMappingForDto(RegisterOneDtoType dtoRegister, MappingProfile readProfile, MappingProfile saveProfile)
        {
            //Now build the mapping using the ConfigGenerator in the register
            dtoRegister.ConfigGenerator.Accessor.AddReadMappingToProfile(readProfile);
            //Only add a mapping if AutoMapper can be used to update/create the entity
            if (dtoRegister.EntityInfo.EntityStyle != EntityStyles.DDDStyled &&
                dtoRegister.EntityInfo.EntityStyle != EntityStyles.ReadOnly)
            {
                dtoRegister.ConfigGenerator.Accessor.AddSaveMappingToProfile(saveProfile);
            }
        }

        private void RegisterDtosInAssemblyAndBuildMaps(Assembly assemblyToScan)
        {
            Header = $"Scanning {assemblyToScan.GetName().Name}";
            var allTypesInAssembly = assemblyToScan.GetTypes();
            var allLinkToEntityClasses = allTypesInAssembly
                .Where(x => x.GetLinkedEntityFromDto(err => AddError(err)) != null);

            if (!IsValid)
                return;
            foreach (var dtoType in allLinkToEntityClasses)
            {
                var dtoRegister = new RegisterOneDtoType(dtoType, allTypesInAssembly, PublicConfig);
                if (!dtoRegister.IsValid)
                {
                    CombineStatuses(dtoRegister);
                    continue;
                }

                SetupMappingForDto(dtoRegister, _readProfile, _saveProfile);
            }
        }


    }
}