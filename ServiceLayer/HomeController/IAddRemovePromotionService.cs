// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericServices;
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