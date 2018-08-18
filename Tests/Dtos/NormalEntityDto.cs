// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class NormalEntityDto : ILinkToEntity<NormalEntity>
    {
        public int Id { get;  set; }
        public string MyString { get; set; }
    }
}