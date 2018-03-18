// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using GenericLibsBase;
using GenericServices.Configuration;
using GenericServices.Extensions.Internal;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Extensions
{
    public static class UnitTestSetup
    {
        public static IStatusGeneric CheckSetupEntities(this DbContext context, bool strictMode = true, IGenericServiceConfig config = null)
        {
            var status = SetupAllEntities.RegisterEntityClasses(context);
            return status;
        }

        public static void AssertSetupEntitiesOk(this DbContext context, bool strictMode = true, IGenericServiceConfig config = null)
        {
            var status = context.CheckSetupEntities(strictMode, config);
            if (status.HasErrors)
            {
                throw new InvalidOperationException($"There were {status.Errors.Count} found for the DbContext {context.GetType().Name}.");
            }
        }

        public static IStatusGeneric CheckSingleDto<TDto>(this DbContext context,  bool strictMode = true, IGenericServiceConfig config = null, Action<string> writeLine = null)
            where TDto : class
        {
            var status = new StatusGenericHandler();
            var linkInterface = typeof(TDto).GetInterface(DecodedDtoExtensions.InterfaceNameILinkToEntity);
            if (linkInterface == null)
            {
                return status.AddError(
                    $"The class {typeof(TDto).Name} does not have the interface {DecodedDtoExtensions.InterfaceNameILinkToEntity} on it. That is required to make GenericServices to work.");
            }

            status.CombineErrors(CheckKnownDtoLinkedToEntity(context, typeof(TDto)));
            return status;
        }

        private static IStatusGeneric CheckKnownDtoLinkedToEntity(this DbContext context, Type dtoType)
        {
            var status = new StatusGenericHandler();
            var entityInfo = context.GetUnderlyingEntityInfo(dtoType);
            var dtoStatus = dtoType.GetOrCreateDtoInfo(entityInfo);
            status.CombineErrors(dtoStatus);
            return status;
        }

        public static IStatusGeneric CheckDtosInAssemblyWith<TDto>(this DbContext context, bool strictMode = true,
            IGenericServiceConfig config = null)
            where TDto : class
        {
            var status = new StatusGenericHandler();
            
            var assemblyToScan = typeof(TDto).Assembly;
            var allLinkToEntityClasses = assemblyToScan.GetTypes()
                .Where(x => x.GetLinkedEntityFromDto() != null);
            var dtoAndentityList = new List<string>();
            foreach (var dtoType in allLinkToEntityClasses)
            {
                var entityType = dtoType.GetLinkedEntityFromDto();
                if (context.Model.GetEntityTypes(entityType) == null)
                {
                    status.AddError(
                        $"The DTO {dtoType.Name} has the entity class {entityType.Name}, which isn't in the context {context.GetType().Name}.");
                }
                status.CombineErrors(context.CheckKnownDtoLinkedToEntity(dtoType));
                dtoAndentityList.Add($"{dtoType.Name}[{entityType.Name}]");
            }

            status.Message = "DTO class+linked entity: " + string.Join(", ", dtoAndentityList);
            return status;
        }
    }
}