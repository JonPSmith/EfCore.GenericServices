// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericServices;
using Tests.EfClasses;

namespace Tests.Dtos
{
    public abstract class DddCtorEntityAbstractBaseDto 
    {
        public int Id { get;  set; }
        public int MyInt { get; set; }
    }
}