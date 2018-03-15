// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericLibsBase;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal static class BuildCall
    {
        public static Action<TIn, TClass> CallMethodReturnVoid<TIn, TClass>(this MethodInfo methodInfo, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(typeof(TIn), "x");
            var pCall = Expression.Parameter(typeof(TClass), "y");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add( Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = Expression.Lambda<Action<TIn, TClass>>(call, false, new[] { pIn, pCall });
            return built.Compile();
        }

        public static Func<TIn, TClass, IStatusGeneric> CallMethodReturnStatus<TIn, TClass>(this MethodInfo methodInfo, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(typeof(TIn), "x");
            var pCall = Expression.Parameter(typeof(TClass), "y");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = Expression.Lambda<Func<TIn, TClass, IStatusGeneric>>(call, false, new[] { pIn, pCall });
            return built.Compile();
        }
    }
}