// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GenericLibsBase
{
    /// <summary>
    /// This contains the error hanlding part of the GenericBizRunner
    /// </summary>
    public class StatusGenericHandler : IStatusGeneric
    {
        private readonly List<ValidationResult> _errors = new List<ValidationResult>();
        private string _successMessage;

        /// <summary>
        /// This holds the list of ValidationResult errors. If the collection is empty, then there were no errors
        /// </summary>
        public IImmutableList<ValidationResult> Errors => _errors.ToImmutableList();

        /// <summary>
        /// This is true if any errors have been reistered 
        /// </summary>
        public bool HasErrors => _errors.Any();

        /// <summary>
        /// On success this returns the message as set by the business logic, or the default messages set by the BizRunner
        /// If there are errors it contains the message "Failed with NN errors"
        /// </summary>
        public string Message
        {
            get => HasErrors
                ? $"Failed with {_errors.Count} error" + (_errors.Count == 1 ? "" : "s")
                : _successMessage ?? "<not run yet>";
            set => _successMessage = value;
        }

        /// <summary>
        /// This adds one error to the Errors collection
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        /// <param name="propertyNames">optional. A list of property names that this error applies to</param>
        public void AddError(string errorMessage, params string[] propertyNames)
        {
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));
            _errors.Add(new ValidationResult
                (errorMessage, propertyNames));
        }

        /// <summary>
        /// This adds one ValidationResult to the Errors collection
        /// </summary>
        /// <param name="validationResult"></param>
        public void AddValidationResult(ValidationResult validationResult)
        {
            _errors.Add(validationResult);
        }

        /// <summary>
        /// This appends a collection of ValidationResults to the Errors collection
        /// </summary>
        /// <param name="validationResults"></param>
        public void AddValidationResults(IEnumerable<ValidationResult> validationResults)
        {
            _errors.AddRange(validationResults);
        }

        /// <summary>
        /// This allows statuses to be combined. Copies over any errors and replaces the Message if the currect message is null
        /// </summary>
        /// <param name="status"></param>
        public void CombineErrors(IStatusGeneric status)
        {
            _errors.AddRange(status.Errors);
            if (!HasErrors && status.Message != null)
                Message = status.Message;
        }
    }
}