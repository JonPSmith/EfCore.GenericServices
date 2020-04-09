// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace Tests.EfClasses
{
    [Owned]
    public class OwnedType
    {
        public int OwnedInt { get; set; }
    }
}