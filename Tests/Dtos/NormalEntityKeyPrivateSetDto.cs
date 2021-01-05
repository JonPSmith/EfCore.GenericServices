// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class NormalEntityKeyPrivateSetDto : ILinkToEntity<NormalEntity>
    {
        public int Id { get; private set; }
        public int MyInt { get; set; }
        public string MyString { get; set; }
    }
}