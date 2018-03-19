// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration;
using GenericServices.Startup.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace GenericServices.Startup
{
    public static class UnitTestSetup
    {
        public static WrappedAutoMapperConfig SetupSingleDtoAndEntities<TDto>(this DbContext context, bool withMapping, IGenericServiceConfig globalConfig = null)
        {
            var status = context.RegisterEntityClasses();
            var dtoRegister = new RegisterOneDtoType(typeof(TDto), globalConfig);
            status.CombineErrors(dtoRegister);
            if (status.HasErrors)
                throw new InvalidOperationException($"SETUP FAILED with {status.Errors.Count}. Errors are:\n" 
                                                    + string.Join("\n", status.Errors.Select(x => x.ToString())));

            if (!withMapping)
                return null;

            MapperConfiguration mapConfig = null;
            var readProfile = new MappingProfile(false);
            dtoRegister.MapGenerator.Accessor.BuildReadMapping(readProfile);
            if (dtoRegister.EntityInfo.EntityStyle == GenericServices.Internal.Decoders.EntityStyles.Normal)
            {
                var saveProfile = new MappingProfile(true);
                dtoRegister.MapGenerator.Accessor.BuildSaveMapping(saveProfile);
                mapConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(readProfile);
                    cfg.AddProfile(saveProfile);
                });
            }
            else
            {
                mapConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(readProfile);
                });
            }

            return new WrappedAutoMapperConfig(mapConfig);
        }
    }
}