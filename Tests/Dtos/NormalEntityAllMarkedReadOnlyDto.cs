// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel;
using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class NormalEntityAllMarkedReadOnlyDto : ILinkToEntity<NormalEntity>
    {
        [ReadOnly(true)]
        public int Id { get;  set; }
        [ReadOnly(true)]
        public string MyString { get; set; }
    }
}