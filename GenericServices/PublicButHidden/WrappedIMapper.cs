// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using AutoMapper;

namespace GenericServices.PublicButHidden
{
    public interface IWrappedIMapper
    {
        IMapper Mapper { get; }
    }

    public class WrappedIMapper : IWrappedIMapper
    {
        public WrappedIMapper(IMapper mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IMapper Mapper { get; }


    }
}