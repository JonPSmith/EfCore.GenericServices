// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController
{
    public interface IListBooksService
    {
        Task<IQueryable<BookListDto>> SortFilterPage
            (SortFilterPageOptions options);
    }
}