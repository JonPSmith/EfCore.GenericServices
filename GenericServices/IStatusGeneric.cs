// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Immutable;

namespace GenericServices
{
    /// <summary>
    /// This is the interface for creating and returning 
    /// </summary>
    public interface IStatusGeneric
    {
        /// <summary>
        /// This holds the list of errors. If the collection is empty, then there were no errors
        /// </summary>
        IImmutableList<ErrorGeneric> Errors { get; }

        /// <summary>
        /// This is true if there are no errors registered
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// On success this returns any message set by GenericServices, or any method that returns a status
        /// If there are errors it contains the message "Failed with NN errors"
        /// </summary>
        string Message { get; }

        /// <summary>
        /// This allows statuses to be combined
        /// </summary>
        /// <param name="status"></param>
        IStatusGeneric CombineStatuses(IStatusGeneric status);

        /// <summary>
        /// This is a simple method to output all the errors as a single string - null if no errors
        /// Useful for feeding back all the errors in a single exception (also nice in unit testing)
        /// </summary>
        /// <param name="separator">if null then each errors is separated by Environment.NewLine, otherwise uses the separator you provide</param>
        /// <returns>a single string with all errors separated by the 'separator' string</returns>
        string GetAllErrors(string separator = null);
    }
}