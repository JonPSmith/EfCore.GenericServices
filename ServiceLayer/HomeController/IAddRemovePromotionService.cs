// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using ServiceLayer.HomeController.Dtos;
using StatusGeneric;

namespace ServiceLayer.HomeController
{
    public interface IAddRemovePromotionService : IStatusGeneric
    {
        AddRemovePromotionDto GetOriginal(int id);
        Book AddPromotion(AddRemovePromotionDto dto);
        Book RemovePromotion(int id);
    }
}