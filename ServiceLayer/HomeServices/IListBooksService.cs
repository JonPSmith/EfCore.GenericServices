// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using ServiceLayer.HomeServices.Dtos;

namespace ServiceLayer.HomeServices
{
    public interface IListBooksService
    {
        IQueryable<BookListDto> SortFilterPage
            (SortFilterPageOptions options);
    }
}