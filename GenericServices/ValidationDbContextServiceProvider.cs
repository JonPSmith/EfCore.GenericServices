// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace GenericServices
{
    /// <summary>
    /// This is the serviceProvider used in the validation of data in SaveChanges
    /// It allows the developer to access the current DbContext in the IValidateableObject
    /// </summary>
    public class ValidationDbContextServiceProvider : IServiceProvider
    {
        private readonly DbContext _currContext;

        /// <summary>
        /// This creates the validation service provider
        /// </summary>
        /// <param name="currContext">The currect DbContext in which this validation is happening</param>
        public ValidationDbContextServiceProvider(DbContext currContext)
        {
            _currContext = currContext;
        }

        /// <summary>
        /// This implemenents the GetService part of the service provider. It only understands the type DbContext
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(DbContext))
            {
                return _currContext;
            }

            return null;
        }
    }
}