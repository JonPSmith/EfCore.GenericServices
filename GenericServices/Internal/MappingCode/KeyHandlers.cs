// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.MappingCode
{
    internal static class KeyHandlers
    {
        public static void CopyBackKeysFromEntityToDtoIfPresent<TDto>(this object newEntity, TDto dto, DecodedEntityClass entityInfo)
        {
            var dtoKeyProperies = typeof(TDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var entityKeys in entityInfo.PrimaryKeyProperties)
            {
                var dtoMatchingProperty =
                    dtoKeyProperies.SingleOrDefault(
                        x => x.Name == entityKeys.Name && x.PropertyType == entityKeys.PropertyType);
                if (dtoMatchingProperty != null //found one
                    && dtoMatchingProperty.CanWrite //CanWrite
                    && dtoMatchingProperty.SetMethod.IsPublic)  //setter is public
                    dtoMatchingProperty.SetValue(dto, entityKeys.GetValue(newEntity));
            }
        }

        /// <summary>
        /// This extracts the key values from the DTO
        /// We can't be sure that the DecodedDto holds the keys in the correct order, so we use EF Core to provide them
        /// </summary>
        /// <typeparam name="TDto"></typeparam>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="dtoInfo"></param>
        /// <returns></returns>
        public static object[] GetKeysFromDtoInCorrectOrder<TDto>(this DbContext context, TDto dto, DecodedDto dtoInfo)
            where TDto : class
        {
            var keys = new List<object>();

            foreach (var entityKey in context.Model.FindEntityType(dtoInfo.LinkedEntityInfo.EntityType).FindPrimaryKey().Properties)
            {
                var dtoKeyProperty = dtoInfo.PropertyInfos.SingleOrDefault(x => x.PropertyInfo.Name == entityKey.Name);
                if (dtoKeyProperty == null)
                    throw new InvalidOperationException($"The DTO/VM class {typeof(TDto).Name} does not contain the primary key {entityKey.Name}."+
                                                        " You have to include every part of a primary key in the DTO for this service to work.");
                keys.Add(dtoKeyProperty.PropertyInfo.GetValue(dto));
            }

            return keys.ToArray();
        }
    }
}