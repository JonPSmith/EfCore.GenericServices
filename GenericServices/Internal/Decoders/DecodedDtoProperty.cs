// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;

namespace GenericServices.Internal.Decoders
{
    [Flags]
    internal enum DtoPropertyTypes
    {
        Normal = 0,
        //ReadOnly means that an update to a entity using AutoMapper will NOT copy this property back
        ReadOnly = 1,
        //This means the property matches a property in the entity class taht is part of the Key, and should be copied back after a create
        KeyProperty = 2
    }
    internal class DecodedDtoProperty
    {
        public DecodedDtoProperty(PropertyInfo propertyInfo, bool isKeyProperty)
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            //NOTE: Key properties are deemed to by read-only, as we don't want to set them
            PropertyType = isKeyProperty ? DtoPropertyTypes.KeyProperty | DtoPropertyTypes.ReadOnly : DtoPropertyTypes.Normal;
            var readOnlyAttr = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly;
            if (readOnlyAttr ?? false)
                PropertyType |= DtoPropertyTypes.ReadOnly;
        }

        public PropertyInfo PropertyInfo { get; private set; }
        public DtoPropertyTypes PropertyType { get; private set; }
    }
}