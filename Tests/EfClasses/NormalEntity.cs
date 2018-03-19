// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Tests.EfClasses
{
    public class NormalEntity
    {
        public int Id { get; set; }
        public int MyInt { get; set; }
        public string MyString { get; set; }
    }
}