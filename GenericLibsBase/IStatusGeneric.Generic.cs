// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericLibsBase
{
    public interface IStatusGeneric<out T> : IStatusGeneric
    {
        /// <summary>
        /// This contains the return result, or if there are errors it will retunr default(T)
        /// </summary>
        T Result { get; }
    }
}