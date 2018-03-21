// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace GenericServices.Configuration.Internal
{
    public interface IExpandedGlobalConfig
    {
        IGenericServicesConfig PublicConfig { get; }
        DbContext CurrentContext { get; }
        PropertyMatch InternalPropertyMatch(string name, Type type, PropertyInfo propertyInfo);
    }

    public class ExpandedGlobalConfig : IExpandedGlobalConfig
    {
        public IGenericServicesConfig PublicConfig { get; }

        public DbContext CurrentContext { get; }

        public ExpandedGlobalConfig(IGenericServicesConfig publicConfig, DbContext currentContext)
        {
            PublicConfig = publicConfig ?? new GenericServicesConfig();
            CurrentContext = currentContext;
        }

        /// <summary>
        /// This handles method properties that require injection of DbContext 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public PropertyMatch InternalPropertyMatch(string name, Type type, PropertyInfo propertyInfo)
        {
            if (type == typeof(DbContext) || type.IsSubclassOf(typeof(DbContext)))
                return new PropertyMatch(true, PropertyMatch.TypeMatchLevels.Match, null, MatchSources.DbContext);

            return PublicConfig.NameMatcher(name, type, propertyInfo);
        }


    }
}