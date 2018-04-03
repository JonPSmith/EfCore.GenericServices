// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GenericServices.Configuration;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal class BuildCallAsync
    {
        public static dynamic TaskCallMethodReturnVoid(MethodInfo methodInfo, Type tDto, Type tEntity, 
            List<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tDto, "dto");
            var pCall = Expression.Parameter(tEntity, "call");
            var args = new List<Expression>();
            foreach (var propertyMatch in propertyMatches)
            {
                args.Add(Expression.Property(pIn, propertyMatch.PropertyInfo));
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = propertyMatches.Any()
                ? Expression.Lambda(call, false, pIn, pCall)
                : Expression.Lambda(call, false, pCall);
            return built.Compile();
        }
    }
}