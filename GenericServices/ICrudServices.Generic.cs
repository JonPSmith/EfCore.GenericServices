// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace GenericServices
{
    /// <summary>
    /// This is the interface for the form of CrudServices where you define the DbContext to be used in the CrudServices
    /// Useful if you have multiple DbContext (known as database bounded contexts) 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface ICrudServices<TContext> : ICrudServices where TContext : DbContext { }
}