// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal static class BuildCall
    {
        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnVoidCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnVoid(this MethodInfo methodInfo, Type tIn, Type tClass, params PropertyInfo[] properties)
        {
            return CallMethodReturnVoidCache.GetOrAdd(methodInfo.GenerateKey(), 
                type => methodInfo.PrivateCallMethodReturnVoid(tIn, tClass, properties));
        }

        private static dynamic PrivateCallMethodReturnVoid(this MethodInfo methodInfo, Type tIn, Type tClass, params PropertyInfo[] properties)
        {         
            var pIn = Expression.Parameter(tIn, "x");
            var pCall = Expression.Parameter(tClass, "y");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = Expression.Lambda(call, false, pIn, pCall);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnStatusCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnStatus(this MethodInfo methodInfo, Type tIn, Type tClass, params PropertyInfo[] properties)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => methodInfo.PrivateCallMethodReturnStatus(tIn, tClass, properties));
        }

        private static dynamic PrivateCallMethodReturnStatus(this MethodInfo methodInfo, Type tIn, Type tClass, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(tIn, "x");
            var pCall = Expression.Parameter(tClass, "y");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = Expression.Lambda(call, false, pIn, pCall);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallStaticFactoryCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallStaticFactory(this MethodInfo methodInfo, Type tIn, params PropertyInfo[] properties)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => methodInfo.PrivateCallStaticFactory(tIn, properties));
        }

        private static dynamic PrivateCallStaticFactory(this MethodInfo methodInfo, Type tIn, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(tIn, "x");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var call = Expression.Call(null, methodInfo, args);
            var built = Expression.Lambda(call, false, pIn);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallConstructorCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallConstructor(this ConstructorInfo ctor, Type tIn, params PropertyInfo[] properties)
        {
            return CallMethodReturnStatusCache.GetOrAdd(ctor.GenerateKey(),
                type => ctor.PrivateCallConstructor(tIn, properties));
        }

        public static dynamic PrivateCallConstructor(this ConstructorInfo ctor, Type tIn, params PropertyInfo[] properties)
        {
            var pIn = Expression.Parameter(tIn, "x");
            var args = new List<MemberExpression>();
            foreach (var propertyInfo in properties)
            {
                args.Add(Expression.Property(pIn, propertyInfo));
            }
            var newExp = Expression.New(ctor, args);
            var built = Expression.Lambda(newExp, false, pIn);
            return built.Compile();
        }

        private static string GenerateKey(this MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType.FullName + (methodInfo.IsStatic ? "Static" : "") + methodInfo.ToString();
        }

        private static string GenerateKey(this ConstructorInfo ctorInfo)
        {
            return ctorInfo.DeclaringType.FullName + ctorInfo.ToString();
        }
    }
}