// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class DtoLinkedToOwnedType : ILinkToEntity<Address>
    {
        public string Address1 { get; set; }
    }
}