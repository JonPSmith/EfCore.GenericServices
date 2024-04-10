// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;
using GenericServices;

namespace Tests.Dtos
{
    public class AddReviewByNameExtendedDto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }

        [ReadOnly(true)]
        public string Title { get; set; }

        [MaxLength(Review.NameLength)]
        public string VoterName { get; set; }

        public int NumStars { get; set; }
        public string Comment { get; set; }
    }
}