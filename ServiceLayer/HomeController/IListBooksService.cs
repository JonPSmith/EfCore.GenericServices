// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController
{
    public interface IListBooksService
    {
        IQueryable<BookListDto> SortFilterPage
            (SortFilterPageOptions options);
    }
}