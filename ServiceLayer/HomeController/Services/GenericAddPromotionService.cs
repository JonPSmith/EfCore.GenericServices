// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericServices;
using ServiceLayer.HomeController.Dtos;

namespace ServiceLayer.HomeController.Services
{
    public class GenericAddPromotionService 
    {
        private readonly ICrudServices _service;

        public IStatusGeneric Status => _service;

        public GenericAddPromotionService(ICrudServices service)
        {
            _service = service;
        }

        public AddRemovePromotionDto GetOriginal(int id)
        {
            return _service.ReadSingle<AddRemovePromotionDto>(id);
        }

        public void AddPromotion(AddRemovePromotionDto dto)
        {
            _service.UpdateAndSave(dto);
        }
    }
}
