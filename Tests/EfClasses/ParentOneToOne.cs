// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public class ParentOneToOne
    {
        public int Id { get; set; }

        public Child OneToOne { get; set; }
    }
}