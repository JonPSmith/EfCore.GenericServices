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
            var built = Expression.Lambda<Action<TIn, TClass>>(call, false, pIn, pCall);
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
            var built = Expression.Lambda<Func<TIn, TClass, IStatusGeneric>>(call, false, pIn, pCall);
            return built.Compile();
        }

        public static Func<TIn, IStatusGeneric<TClass>> CallStaticFactory<TIn, TClass>(this MethodInfo methodInfo, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(typeof(TIn), "x");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(null, methodInfo, args);
            var built = Expression.Lambda<Func<TIn, IStatusGeneric<TClass>>>(call, false, pIn);
            return built.Compile();
        }

        public static Func<TIn, TClass> CallConstructor<TIn, TClass>(this ConstructorInfo ctor, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(typeof(TIn), "x");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var newExp = Expression.New(ctor, args);
            var built = Expression.Lambda<Func<TIn, TClass>>(newExp, false, pIn);
            return built.Compile();
        }
    }
}