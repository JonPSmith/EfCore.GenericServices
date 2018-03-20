// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace GenericServices
{
    public struct ErrorGeneric
    {
        private const string HeaderSeparator = ">";
        public ErrorGeneric(string header, ValidationResult error) : this()
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            ErrorResult = error ?? throw new ArgumentNullException(nameof(error));
        }

        internal ErrorGeneric(string prefix, ErrorGeneric existingError)
        {          
            Header = string.IsNullOrEmpty(prefix)
                ? existingError.Header
                : prefix + HeaderSeparator + existingError.Header;
            ErrorResult = existingError.ErrorResult;
        }

        public string Header { get; private set; }
        public ValidationResult ErrorResult { get; private set; }

        public override string ToString()
        {
            var start = string.IsNullOrEmpty(Header) ? "" : Header + ": ";
            return start + ErrorResult.ToString();
        }

    }
}