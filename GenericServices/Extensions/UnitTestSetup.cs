// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericLibsBase;
using GenericServices.Extensions.Internal;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Extensions
{
    public static class UnitTestSetup
    {
        public static IStatusGeneric CheckSetupEntities(this DbContext context, bool strictMode = true, IGenericServiceConfig config = null, Action<string> writeLine = null)
        {
            var status = SetupAllEntities.SetupEntityClasses(context);
            if (status.HasErrors)
            {
                if (writeLine != null)
                {
                    foreach (var error in status.Errors)
                    {
                        writeLine.Invoke(error.ToString());
                    }
                }
            }

            return status;
        }

        public static void AssertSetupEntitiesOk(this DbContext context, bool strictMode = true, IGenericServiceConfig config = null, Action<string> writeLine = null)
        {
            var status = context.CheckSetupEntities(strictMode, config, writeLine);
            if (status.HasErrors)
            {
                throw new InvalidOperationException($"There were {status.Errors.Count} found for the DbContext {context.GetType().Name}.");
            }
        }

        public static IStatusGeneric CheckSingleDto<TDto>(this DbContext context,  bool strictMode = true, IGenericServiceConfig config = null, Action<string> writeLine = null)
            where TDto : class
        {
            var status = new StatusGenericHandler();
            var linkInterface = typeof(TDto).GetInterface(DecodedDto.NameILinkToEntity);
            if (linkInterface == null)
            {
                return status.AddError(
                    $"The class {typeof(TDto).Name} does not have the interface {DecodedDto.NameILinkToEntity} on it. That is required to make GenericServices to work.");
            }

            CheckKnownDtoLinkedToEntity(context, typeof(TDto), writeLine, status);
            return status;
        }

        private static void CheckKnownDtoLinkedToEntity(this DbContext context, Type dtoType, Action<string> writeLine, StatusGenericHandler status)
        {
            var entityInfo = context.GetUnderlyingEntityInfo(dtoType);
            var dtoStatus = dtoType.GetDtoInfo(entityInfo);
            status.CombineErrors(dtoStatus);
            if (status.HasErrors)
            {
                if (writeLine != null)
                {
                    foreach (var error in status.Errors)
                    {
                        writeLine.Invoke(error.ToString());
                    }
                }
            }
        }

        public static IStatusGeneric CheckDtosInAssemblyWith<TDto>(this DbContext context, bool strictMode = true,
            IGenericServiceConfig config = null, Action<string> writeLine = null)
            where TDto : class
        {
            var status = new StatusGenericHandler();
            var assemblyToScan = typeof(TDto).Assembly;
            var allLinkToEntityClasses = assemblyToScan.GetTypes()
                .Where(x => x.GetLinkInterfaceFromDto() != null);
            foreach (var dtoType in allLinkToEntityClasses)
            {
                var entityType = dtoType.GetLinkInterfaceFromDto();
                if (context.Model.GetEntityTypes(entityType) == null)
                {
                    status.AddError(
                        $"The DTO {dtoType.Name} has the entity class {entityType.Name}, which isn't in the context {context.GetType().Name}.");
                }
                context.CheckKnownDtoLinkedToEntity(dtoType, writeLine, status);
            }

            return status;
        }
    }
}