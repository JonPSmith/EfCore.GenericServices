// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Tests.EfClasses
{
    public class ParentOneToOne
    {
        public int Id { get; set; }

        public Child OneToOne { get; set; }

        public void AddSpace(Space space)
        {

        }

        public void AddFacilities(IEnumerable<string> facilities)
        {
        }
    }

    public class Space{}
}