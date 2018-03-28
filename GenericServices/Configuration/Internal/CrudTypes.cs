// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace GenericServices.Configuration.Internal
{
    [Flags]
    internal enum CrudTypes
    {
        None = 0,
        Create = 1,
        ReadOne = 2,
        ReadMany = 4,
        Update = 8,
        Delete = 16
    }
}