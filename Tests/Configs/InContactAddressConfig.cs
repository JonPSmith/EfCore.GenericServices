// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;
using GenericServices.Configuration;
using Tests.Dtos;
using Tests.EfClasses;

namespace Tests.Configs
{
    public class InContactAddressConfig : PerDtoConfig<InContactAddressDto, ContactAddress>
    {
        public override Action<IMappingExpression<InContactAddressDto, ContactAddress>> AlterSaveMapping
        {
            get
            {
                return cfg => cfg.ForMember(d => d.Address, opt => opt.MapFrom(src => src.Addess));
            }
        }
    }
}