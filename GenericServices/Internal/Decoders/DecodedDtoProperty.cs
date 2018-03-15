// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;

namespace GenericServices.Internal.Decoders
{
    public class DecodedDtoProperty
    {
        public PropertyInfo PropertyInfo { get; private set; }
        public bool ReadOnlyAttribute { get; private set; }

        public DecodedDtoProperty(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            ReadOnlyAttribute = propertyInfo.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false;
        }

    }
}