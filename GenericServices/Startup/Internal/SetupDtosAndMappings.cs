// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;

namespace GenericServices.Startup.Internal
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

        public IWrappedAutoMapperConfig ScanAllAssemblies(Assembly[] assembliesToScan, bool initializeMapper)
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

            if (initializeMapper)
            {
                Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile(_readProfile);
                    cfg.AddProfile(_saveProfile);
                });
            }
            return CreateWrappedAutoMapperConfig(_readProfile, _saveProfile);
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