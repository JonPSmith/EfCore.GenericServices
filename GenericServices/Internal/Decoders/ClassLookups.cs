// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericServices.PublicButHidden;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Internal.Decoders
{
    internal static class ClassLookups
    {
        public static DecodedEntityClass GetUnderlyingEntityInfo(this Type entityOrDto, DbContext context)
        {
            var entityType = entityOrDto.IsSubclassOf(typeof(GenericServiceBaseDto))
                ? ReturnEntityClassAssociatedWithDto(entityOrDto)
                : entityOrDto;
            if (entityType == null)
                throw new InvalidOperationException($"The class {entityOrDto.Name} was not found in the {context.GetType().Name} DbContext." +
                                                    " The class must be either be an entity class derived from the GenericServiceDto/Async class.");
            return entityType.GetEntityClassInfo(context);
        }

        private static Type ReturnEntityClassAssociatedWithDto(Type entityOrDto)
        {
            return GetGenericTypesIfCorrectGeneric(entityOrDto, typeof(GenericServiceBaseDto<,>))?[0];

        }


        private static Type[] GetGenericTypesIfCorrectGeneric(Type classType, Type genericDtoClass)
        {
            while (classType.Name != genericDtoClass.Name && classType.BaseType != null)
                classType = classType.BaseType;

            return classType.Name != genericDtoClass.Name ? null : classType.GetGenericArguments();
        }


    }
}