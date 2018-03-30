// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel;
using DataLayer.EfClasses;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace ServiceLayer.HomeController.Dtos
{
    public class AddRemovePromotionDto : ILinkToEntity<Book>
    {
        [HiddenInput]
        public int BookId { get; set; }

        [ReadOnly(true)]
        public decimal OrgPrice { get; set; }

        [ReadOnly(true)]
        public string Title { get; set; }

        public decimal ActualPrice { get; set; }

        //This would normally added to give feedback at the UI level, but I wanted the business logic to show
        //[Required(AllowEmptyStrings = false)]
        public string PromotionalText { get; set; }
    }

}