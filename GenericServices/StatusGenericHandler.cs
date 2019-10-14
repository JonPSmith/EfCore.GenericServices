// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GenericServices
{
    /// <summary>
    /// This contains the error hanlding part of the GenericBizRunner
    /// </summary>
    public class StatusGenericHandler : IStatusGeneric
    {
        internal const string DefaultSuccessMessage = "Success";
        private readonly List<ErrorGeneric> _errors = new List<ErrorGeneric>();
        private string _successMessage = DefaultSuccessMessage;

        /// <summary>
        /// This creates a StatusGenericHandler, with optional header (see Header property, and CombineStatuses)
        /// </summary>
        /// <param name="header"></param>
        public StatusGenericHandler(string header = "")
        {
            Header = header;
        }

        /// <summary>
        /// This holds the list of ValidationResult errors. If the collection is empty, then there were no errors
        /// </summary>
        public IImmutableList<ErrorGeneric> Errors => _errors.ToImmutableList();

        /// <summary>
        /// This is true if any errors have been reistered 
        /// </summary>
        public bool IsValid => !_errors.Any();

        /// <summary>
        /// On success this returns the message as set by the business logic, or the default messages set by the BizRunner
        /// If there are errors it contains the message "Failed with NN errors"
        /// </summary>
        public string Message
        {
            get => IsValid
                ? _successMessage
                : $"Failed with {_errors.Count} error" + (_errors.Count == 1 ? "" : "s");
            set => _successMessage = value;
        }

        /// <summary>
        /// The header provides a prefix to any errors you add. Useful if you want to have a general prefix to all your errors
        /// e.g. a header if "MyClass" would produce error messages such as "MyClass: This is my error message."
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// This adds one error to the Errors collection
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        /// <param name="propertyNames">optional. A list of property names that this error applies to</param>
        public IStatusGeneric AddError(string errorMessage, params string[] propertyNames)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            _errors.Add(new ErrorGeneric(Header, new ValidationResult(errorMessage, propertyNames)));
            return this;
        }

        /// <summary>
        /// This adds one ValidationResult to the Errors collection
        /// </summary>
        /// <param name="validationResult"></param>
        public void AddValidationResult(ValidationResult validationResult)
        {
            _errors.Add(new ErrorGeneric(Header, validationResult));
        }

        /// <summary>
        /// This appends a collection of ValidationResults to the Errors collection
        /// </summary>
        /// <param name="validationResults"></param>
        public void AddValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            _errors.AddRange(validationResults.Select(x => new ErrorGeneric(Header, x)));
        }

        /// <summary>
        /// This allows statuses to be combined. Copies over any errors and replaces the Message if the currect message is null
        /// If you are using Headers then it will combine the headers in any errors in combines
        /// e.g. Status1 with header "MyClass" combines Status2 which has header "MyProp" and status2 has errors.
        /// The result would be error message in status2 would be updates to start with "MyClass>MyProp: This is my error message."
        /// </summary>
        /// <param name="status"></param>
        public IStatusGeneric CombineStatuses(IStatusGeneric status)
        {
            if (!status.IsValid)
            {
                _errors.AddRange(string.IsNullOrEmpty(Header)
                    ? status.Errors
                    : status.Errors.Select(x => new ErrorGeneric(Header, x)));
            }
            if (IsValid && status.Message != DefaultSuccessMessage)
                Message = status.Message;

            return this;
        }

        /// <summary>
        /// This is a simple method to output all the errors as a single string - null if no errors
        /// Useful for feeding back all the errors in a single exception (also nice in unit testing)
        /// </summary>
        /// <param name="separator">if null then each errors is separated by Environment.NewLine, otherwise uses the separator you provide</param>
        /// <returns>a single string with all errors separated by the 'separator' string</returns>
        public string GetAllErrors(string separator = null)
        {
            separator = separator ?? Environment.NewLine;
            return _errors.Any() 
                ? string.Join(separator, Errors) 
                : null;
        }
    }
}