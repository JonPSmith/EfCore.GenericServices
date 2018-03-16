// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using GenericServices.Internal.Decoders;

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
                if (dtoMatchingProperty == null) continue;

                dtoMatchingProperty.SetValue(dto, entityKeys.GetValue(newEntity));
            }
        }
    }
}