// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices
{
    /// <summary>
    /// This allows you to link a class to an entity class. GenericServices then uses that to map your dto class to the entity class
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface ILinkToEntity<TEntity> where TEntity : class {}
}