// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using GenericServices.Configuration;
using GenericServices.Configuration.Internal;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;
using Microsoft.Extensions.DependencyInjection;

namespace GenericServices.Startup.Internal
{
    internal class SetupAllDtosAndMappings : StatusGenericHandler, IGenericServicesSetupPart2
    {
        private IGenericServicesConfig _publicConfig;
        private Type[] _contextTypes;

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
        }


        public void RegisterDtosInAssemblyAndBuildMaps(Assembly assemblyToScan)
        {
            Header = $"Scanning {assemblyToScan.GetName()}";
            var allTypesInAssembly = assemblyToScan.GetTypes();
            var allLinkToEntityClasses = allTypesInAssembly
                .Where(x => x.GetLinkedEntityFromDto() != null);

            foreach (var dtoType in allLinkToEntityClasses)
            {
                var register = new RegisterOneDtoType(dtoType, _publicConfig);
                if (!register.IsValid)
                {
                    CombineStatuses(register);
                    continue;
                }

                //Now build the mapping using the MapGenerator in the register
            }

            //Now scan for next maps and set up the mapping for them too
            throw new NotImplementedException();
            //Don't forget to look at the TurnOffAuthoMapperSaveFilter in the GenericServicesConfig
        }



    }
}