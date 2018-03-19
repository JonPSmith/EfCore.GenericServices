// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Helpers
{
    public class UnitTestProfile : Profile
    {
        public UnitTestProfile(bool addIgnoreParts)
        {
            if (addIgnoreParts)
                ForAllPropertyMaps(pm => Filter(pm.SourceMember), (pm, opt) => opt.Ignore());
        }

        public void AddReadMap<TIn, TOut>(Action<IMappingExpression<TIn, TOut>> alterMapping = null)
        {
            if (alterMapping == null)
                CreateMap<TIn, TOut>();
            else
                alterMapping(CreateMap<TIn, TOut>());
        }

        public void AddWriteMap<TIn, TOut>(Action<IMappingExpression<TIn, TOut>> alterMapping = null)
        {
            if (alterMapping == null)
                CreateMap<TIn, TOut>().IgnoreAllPropertiesWithAnInaccessibleSetter();
            else
                alterMapping(CreateMap<TIn, TOut>().IgnoreAllPropertiesWithAnInaccessibleSetter());
        }

        private bool Filter(MemberInfo member)
        {
            if (member.GetCustomAttribute<UIHintAttribute>()?.UIHint == "Hidden")
                return true;
            var readOnlyAttr = member.GetCustomAttribute<ReadOnlyAttribute>();
            var isReadOnly = readOnlyAttr?.IsReadOnly ?? false;
            return isReadOnly;
        }
    }
}