// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using AutoMapper;
using DataLayer.EfClasses;
using GenericServices.Configuration;
using Tests.Dtos;

namespace Tests.Configs
{
    public class AuthorNestedV2DtoConfig : PerDtoConfig<AuthorNestedV2Dto, BookAuthor>
    {
        public override Action<IMappingExpression<BookAuthor, AuthorNestedV2Dto>> AlterReadMapping
        {
            get { return cfg => cfg.ForMember(x => x.AStringToHoldAuthorName, 
                x => x.MapFrom(y => y.Author.Name)); }
        }
    }
}