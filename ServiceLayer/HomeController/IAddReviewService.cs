// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericLibsBase;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController
{
    public interface IAddReviewService : IStatusGeneric
    {
        AddReviewDto GetOriginal(int id) ;

        Book AddReviewToBook(AddReviewDto dto);
    }
}