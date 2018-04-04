// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices
{
    /// <summary>
    /// This is a version of <see cref="IStatusGeneric"/> that contains a result.
    /// Useful if you want to return something with the status
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IStatusGeneric<out T> : IStatusGeneric
    {
        /// <summary>
        /// This contains the return result, or if there are errors it will retunr default(T)
        /// </summary>
        T Result { get; }
    }
}