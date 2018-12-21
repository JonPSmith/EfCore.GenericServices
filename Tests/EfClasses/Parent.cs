// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;

namespace Tests.EfClasses
{
    public class Parent
    {
        public int ParentId { get; set; }
        public ICollection<Child> Children { get; set; }
    }
}