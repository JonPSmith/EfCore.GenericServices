// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Tests.EfClasses
{
    public class UniqueEntity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string UniqueString { get; set; }
    }
}