// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace ServiceLayer.HomeController.Dtos
{
    public class AddRemovePromotionDto 
    {
        public int BookId { get; set; }
        public decimal OrgPrice { get; set; }
        public string Title { get; set; }

        public decimal ActualPrice { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string PromotionalText { get; set; }
    }

}