// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using GenericLibsBase;
using GenericServices.Configuration;
using GenericServices.Internal.Decoders;
using GenericServices.PublicButHidden;

namespace GenericServices.Startup.Internal
{
    internal class SetupAllDtosAndMappings : StatusGenericHandler
    {
        private readonly IGenericServiceConfig _configuration;

        WrappedAutoMapperConfig AutoMapperConfig { get; set; }

        public SetupAllDtosAndMappings(IGenericServiceConfig configuration, Assembly[] assembliesToScan)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            //Add ForAllPropertyMaps to start the Mapping
            foreach (var assembly in assembliesToScan)
            {
                RegisterDtosInAssemblyAndBuildMaps(assembly);
            }
        }


        public void RegisterDtosInAssemblyAndBuildMaps(Assembly assemblyToScan)
        {
            var allTypesInAssembly = assemblyToScan.GetTypes();
            var allLinkToEntityClasses = allTypesInAssembly
                .Where(x => x.GetLinkedEntityFromDto() != null);

            foreach (var dtoType in allLinkToEntityClasses)
            {
                var register = new RegisterOneDtoType(dtoType, _configuration);
                if (register.HasErrors)
                {
                    CombineErrors(register);
                    continue;
                }

                //Now build the mapping using the MapGenerator in the register
            }

            //Now scan for next maps and set up the mapping for them too
            throw new NotImplementedException();
        }




    }
}