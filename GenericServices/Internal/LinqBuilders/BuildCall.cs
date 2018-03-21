// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericServices.Configuration;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal static class BuildCall
    {
        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnVoidCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnVoid(this MethodInfo methodInfo, Type tIn, Type tClass, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnVoidCache.GetOrAdd(methodInfo.GenerateKey(), 
                type => methodInfo.PrivateCallMethodReturnVoid(tIn, tClass, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnVoid(this MethodInfo methodInfo, Type tIn, Type tClass, IEnumerable<PropertyMatch> propertyMatches)
        {         
            var pIn = Expression.Parameter(tIn, "dto");
            var pCall = Expression.Parameter(tClass, "method");
            ParameterExpression pContext = null;
            var args = new List<Expression>();
            foreach (var propertyMatch in propertyMatches)
            {
                if (propertyMatch.MatchSource == MatchSources.Property)
                    args.Add(Expression.Property(pIn, propertyMatch.PropertyInfo));
                else
                {
                    pContext = Expression.Parameter(propertyMatch.NonPropertyMatchType, "db");
                    args.Add(pContext);
                }
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = pContext == null
                ? Expression.Lambda(call, false, pIn, pCall)
                : Expression.Lambda(call, false, pIn, pCall, pContext);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnStatusCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnStatus(this MethodInfo methodInfo, Type tIn, Type tClass, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => methodInfo.PrivateCallMethodReturnStatus(tIn, tClass, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnStatus(this MethodInfo methodInfo, Type tIn, Type tClass, IEnumerable<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tIn, "dto");
            var pCall = Expression.Parameter(tClass, "call");
            ParameterExpression pContext = null;
            var args = new List<Expression>();
            foreach (var propertyMatch in propertyMatches)
            {
                if (propertyMatch.MatchSource == MatchSources.Property)
                    args.Add(Expression.Property(pIn, propertyMatch.PropertyInfo));
                else
                {
                    pContext = Expression.Parameter(propertyMatch.NonPropertyMatchType, "db");
                    args.Add(pContext);
                }
            }
            var call = Expression.Call(pCall, methodInfo, args);
            var built = pContext == null
                ? Expression.Lambda(call, false, pIn, pCall)
                : Expression.Lambda(call, false, pIn, pCall, pContext);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallStaticFactoryCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallStaticFactory(this MethodInfo methodInfo, Type tIn, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => methodInfo.PrivateCallStaticFactory(tIn, propertyMatches));
        }

        private static dynamic PrivateCallStaticFactory(this MethodInfo methodInfo, Type tIn, IEnumerable<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tIn, "dto");
            ParameterExpression pContext = null;
            var args = new List<Expression>();
            foreach (var propertyMatch in propertyMatches)
            {
                if (propertyMatch.MatchSource == MatchSources.Property)
                    args.Add(Expression.Property(pIn, propertyMatch.PropertyInfo));
                else
                {
                    pContext = Expression.Parameter(propertyMatch.NonPropertyMatchType, "db");
                    args.Add(pContext);
                }
            }
            var call = Expression.Call(null, methodInfo, args);
            var built = pContext == null
                ? Expression.Lambda(call, false, pIn)
                : Expression.Lambda(call, false, pIn, pContext);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallConstructorCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallConstructor(this ConstructorInfo ctor, Type tIn, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(ctor.GenerateKey(),
                type => ctor.PrivateCallConstructor(tIn, propertyMatches));
        }

        public static dynamic PrivateCallConstructor(this ConstructorInfo ctor, Type tIn, IEnumerable<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tIn, "dto");
            ParameterExpression pContext = null;
            var args = new List<Expression>();
            foreach (var propertyMatch in propertyMatches)
            {
                if (propertyMatch.MatchSource == MatchSources.Property)
                    args.Add(Expression.Property(pIn, propertyMatch.PropertyInfo));
                else
                {
                    pContext = Expression.Parameter(propertyMatch.NonPropertyMatchType, "db");
                    args.Add(pContext);
                }
            }
            var newExp = Expression.New(ctor, args);
            var built = pContext == null
                ? Expression.Lambda(newExp, false, pIn)
                : Expression.Lambda(newExp, false, pIn, pContext);
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