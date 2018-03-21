// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Startup.Internal
{
    internal class MappingProfile : Profile
    {
        public MappingProfile(bool addIgnoreParts)
        {
            if (addIgnoreParts)
                ForAllPropertyMaps(pm => Filter(pm.SourceMember), (pm, opt) => opt.Ignore());
        }

        private static bool Filter(MemberInfo member)
        {
            return member.GetCustomAttribute<ReadOnlyAttribute>()?.IsReadOnly ?? false;
        }
    }
}