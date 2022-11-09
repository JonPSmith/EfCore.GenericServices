// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;
using AutoMapper;
using AutoMapper.Internal;

namespace Tests.Helpers
{
    public class UnitTestProfile : Profile
    {
        public UnitTestProfile(bool addIgnoreParts)
        {
            if (addIgnoreParts)
                this.Internal().ForAllPropertyMaps(pm => Filter(pm.SourceMember), (pm, opt) => opt.Ignore());
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
            var readOnlyAttr = member.GetCustomAttribute<ReadOnlyAttribute>();
            var isReadOnly = readOnlyAttr?.IsReadOnly ?? false;
            return isReadOnly;
        }
    }
}