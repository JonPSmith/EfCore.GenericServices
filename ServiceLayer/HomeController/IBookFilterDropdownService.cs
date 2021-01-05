// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using ServiceLayer.HomeController.QueryObjects;

namespace ServiceLayer.HomeController
{
    public interface IBookFilterDropdownService
    {
        IEnumerable<DropdownTuple> GetFilterDropDownValues(BooksFilterBy filterBy);
    }
}