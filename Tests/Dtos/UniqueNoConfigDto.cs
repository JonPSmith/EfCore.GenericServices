// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class UniqueNoConfigDto : ILinkToEntity<UniqueEntity>
    {
        public int Id { get; set; }
        public string UniqueString { get; set; }
    }
}