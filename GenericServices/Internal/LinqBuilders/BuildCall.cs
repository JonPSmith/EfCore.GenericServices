// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace GenericServices.Internal.LinqBuilders
{
    internal class BuildCall
    {
        public Action<object,object> CallMethodReturnVoid(Type tIn, Type tClass, MethodInfo methodInfo, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(tIn, "x");
            var pCall = Expression.Parameter(tClass, "y");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add( Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = Expression.Lambda<Action<object,object>>(call, false, new[] { pIn, pCall });
            return built.Compile();
        }
    }
}