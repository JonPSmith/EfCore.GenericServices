// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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