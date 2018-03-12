// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericLibsBase;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController
{
    public interface IAddRemovePromotionService
    {
        IStatusGeneric Status { get; }

        AddRemovePromotionDto GetOriginal(int id);

        Book AddPromotion(AddRemovePromotionDto dto);
        Book RemovePromotion(int id);
    }
}