// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataLayer.EfClasses;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace ServiceLayer.HomeController.Dtos
{
    public class ChangePubDateDto : ILinkToEntity<Book>
    {
        [HiddenInput]
        public int BookId { get; set; }

        [ReadOnly(true)]
        public string Title { get; set; }

        [DataType(DataType.Date)]               
        public DateTime PublishedOn { get; set; }
    }
}