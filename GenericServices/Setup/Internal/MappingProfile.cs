// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Setup.Internal
{
    internal class MappingProfile : Profile
    {
        public MappingProfile(bool addIgnoreParts)
        {
            if (addIgnoreParts)
                ForAllPropertyMaps(pm => Filter(pm.SourceMember), (pm, opt) => opt.Ignore());
        }

        /// <summary>
        /// This returns true for source properties that we DON'T want to be copied
        /// This stops DTP properties that are null, or have a [ReadOnly(true)] attribute, fom being copied.
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        private static bool Filter(MemberInfo member)
        {         
            return member == null || (member.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false);
        }
    }
}