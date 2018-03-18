// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using GenericServices.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GenericServices.Internal.Decoders
{
    internal static class DecodedDtoExtensions
    {
        private class ClassWithILinkInterface : ILinkToEntity<ClassWithILinkInterface> { }
        //This contains the name of the ILinkToEntity<T> interface
        public static readonly string InterfaceNameILinkToEntity = typeof(ClassWithILinkInterface).GetInterfaces().Single().Name;
        public static readonly string HumanReadableILinkToEntity =
            InterfaceNameILinkToEntity.Substring(0, InterfaceNameILinkToEntity.Length - 2);

        private class ClassWithIConfigInterface : IConfigFoundIn<ClassWithIConfigInterface> { }
        public static readonly string InterfaceNameIConfigFoundIn = typeof(ClassWithIConfigInterface).GetInterfaces().Single().Name;
        public static readonly string HumanReadableIConfigFoundIn =
            InterfaceNameIConfigFoundIn.Substring(0, InterfaceNameILinkToEntity.Length - 2);

        public static Type GetLinkedEntityFromDto(this Type entityOrDto)
        {
            var linkInterface = entityOrDto.GetInterface(InterfaceNameILinkToEntity);
            return linkInterface?.GetGenericArguments().Single();
        }

        public static Type GetConfigFromDto(this Type entityOrDto)
        {
            var linkInterface = entityOrDto.GetInterface(InterfaceNameIConfigFoundIn);
            return linkInterface?.GetGenericArguments().Single();
        }
    }
}