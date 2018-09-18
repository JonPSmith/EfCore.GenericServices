// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public class DddCtorEntityAbstractMainDto : DddCtorEntityAbstractBaseDto, ILinkToEntity<DddCtorEntity>
    {
        public string MyString { get; set; }
    }
}