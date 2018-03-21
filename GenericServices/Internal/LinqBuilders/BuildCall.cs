// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using GenericServices.Configuration;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal static class BuildCall
    {
        public static IStatusGeneric RunMethodViaLinq(this MethodInfo methodInfo, dynamic dto, dynamic entity,
            ImmutableList<PropertyMatch> propertyMatches, DbContext context)
        {
            if (methodInfo.ReturnType == typeof(IStatusGeneric))
            {
                var func = CallMethodReturnStatus(methodInfo, dto.GetType(), entity.GetType(), propertyMatches);
                return propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext)
                    ? (IStatusGeneric)func(dto, entity, context)
                    : (IStatusGeneric)func(dto, entity);
            }

            //Otherwise its an action
            var action = CallMethodReturnVoid(methodInfo, dto.GetType(), entity.GetType(), propertyMatches);
            if (propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext))
                action(dto, entity, context);
            else
                action(dto, entity);
            return new StatusGenericHandler();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnVoidCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnVoid(this MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnVoidCache.GetOrAdd(methodInfo.GenerateKey(), 
                type => methodInfo.PrivateCallMethodReturnVoid(tDto, tEntity, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnVoid(this MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
        {         
            var pIn = Expression.Parameter(tDto, "dto");
            var pCall = Expression.Parameter(tEntity, "method");
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

        public static dynamic CallMethodReturnStatus(this MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => methodInfo.PrivateCallMethodReturnStatus(tDto, tEntity, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnStatus(this MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tDto, "dto");
            var pCall = Expression.Parameter(tEntity, "call");
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

        public static dynamic CallStaticFactory(this MethodInfo methodInfo, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => methodInfo.PrivateCallStaticFactory(tDto, propertyMatches));
        }

        private static dynamic PrivateCallStaticFactory(this MethodInfo methodInfo, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tDto, "dto");
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

        public static dynamic CallConstructor(this ConstructorInfo ctor, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(ctor.GenerateKey(),
                type => ctor.PrivateCallConstructor(tDto, propertyMatches));
        }

        public static dynamic PrivateCallConstructor(this ConstructorInfo ctor, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
        {
            var pIn = Expression.Parameter(tDto, "dto");
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