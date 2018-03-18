// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices.Configuration
{
    /// <summary>
    /// This allows you to add specific configuration to a DTO via an class that contains GenericServices config information
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConfigFoundIn<T> where T : class { } 

}