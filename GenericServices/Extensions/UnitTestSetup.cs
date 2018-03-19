// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration;
using GenericServices.Extensions.Internal;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Extensions
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

            if (withMapping)
                throw new NotImplementedException("Mapping has not been implemnted");

            return null;
        }
    }
}