// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace GenericServices
{
    /// <summary>
    /// In the cases where you have nested classes in an AutoMapper projection you need to tell AutoMapper how to map that nested class. 
    /// This interface tells GenericServices that this class needs to be added to the AutoMapper's mappings
    /// NOTE: a nested map can also have a IConfigFoundIn interface if it needs to alter AutoMapper Read mapping
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface INestedMap<TEntity> where TEntity : class {}
}