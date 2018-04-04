// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace GenericServices
{
    /// <summary>
    /// This holds an error registered in the <see cref="IStatusGeneric"/> Errors collection
    /// </summary>
    public struct ErrorGeneric
    {
        /// <summary>
        /// If there are multiple headers this separator is placed between them, e.g. Update>Author
        /// </summary>
        public const string HeaderSeparator = ">";

        /// <summary>
        /// This ctor will create an ErrorGeneric
        /// </summary>
        /// <param name="header"></param>
        /// <param name="error"></param>
        public ErrorGeneric(string header, ValidationResult error) : this()
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            ErrorResult = error ?? throw new ArgumentNullException(nameof(error));
        }

        internal ErrorGeneric(string prefix, ErrorGeneric existingError)
        {          
            Header = string.IsNullOrEmpty(prefix)
                ? existingError.Header
                : string.IsNullOrEmpty(existingError.Header) 
                    ? prefix
                    : prefix + HeaderSeparator + existingError.Header;
            ErrorResult = existingError.ErrorResult;
        }

        /// <summary>
        /// A Header. Can be null
        /// </summary>
        public string Header { get; private set; }

        /// <summary>
        /// This is the error provided
        /// </summary>
        public ValidationResult ErrorResult { get; private set; }

        /// <summary>
        /// A human-readable error display
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var start = string.IsNullOrEmpty(Header) ? "" : Header + ": ";
            return start + ErrorResult.ToString();
        }

    }
}