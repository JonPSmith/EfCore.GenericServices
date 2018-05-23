// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using GenericServices.Configuration;

namespace GenericServices.Internal.Decoders
{
    internal static class DecodedDtoExtensions
    {
        private class ClassWithILinkInterface : ILinkToEntity<ClassWithILinkInterface> { }
        //This contains the name of the ILinkToEntity<T> interface
        public static readonly string InterfaceNameILinkToEntity = typeof(ClassWithILinkInterface).GetInterfaces().Single().Name;
        public static readonly string HumanReadableILinkToEntity =
            InterfaceNameILinkToEntity.Substring(0, InterfaceNameILinkToEntity.Length - 2);

        public static Type GetLinkedEntityFromDto(this Type entityOrDto, Action<string> addError = null)
        {
            try
            {
                var linkInterface = entityOrDto.GetInterface(InterfaceNameILinkToEntity);
                return linkInterface?.GetGenericArguments().Single();
            }
            catch (AmbiguousMatchException)
            {
                var message =
                    $"You had multiple {HumanReadableILinkToEntity} interfaces on the DTO/VM {entityOrDto.Name}. That isn't allowed.";
                if (addError == null)
                    throw new InvalidOperationException(message);

                addError.Invoke(message);
                return null;
            }
        }

        public static Type FormPerDtoConfigType(this Type dtoType, Type entityType)
        {
            var perDtoConfigBase = typeof(PerDtoConfig<,>);
            Type[] typeArgs = { dtoType, entityType };
            return perDtoConfigBase.MakeGenericType(typeArgs);
        }
    }
}