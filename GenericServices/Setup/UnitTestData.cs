// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.Configuration;
using GenericServices.PublicButHidden;
using GenericServices.Setup.Internal;

namespace GenericServices.Setup
{
    public class UnitTestData
    { 
        internal MappingProfile ReadProfile { get; }
        internal MappingProfile SaveProfile { get; }

        public IWrappedConfigAndMapper ConfigAndMapper => SetupDtosAndMappings.CreateConfigAndMapper(PublicConfig, ReadProfile, SaveProfile);

        public IGenericServicesConfig PublicConfig { get; }
    
        public UnitTestData(IGenericServicesConfig publicConfig)
        {
            PublicConfig = publicConfig ?? new GenericServicesConfig();
            ReadProfile = new MappingProfile(false);
            SaveProfile = new MappingProfile(true);
        }
    }
}