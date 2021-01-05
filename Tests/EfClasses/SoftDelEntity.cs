// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Tests.EfClasses
{
    public class SoftDelEntity
    {
        public int Id { get; set; }

        public bool SoftDeleted { get; set; }
    }
}