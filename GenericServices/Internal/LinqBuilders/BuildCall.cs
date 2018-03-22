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
using GenericServices.Internal.Decoders;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal static class BuildCall
    {
        //BE WARNED: extension methods don't work with dynamics - got some strange error messages
        //Also the dto and entity properties cannit be object, but dynamic works

        public static IStatusGeneric RunMethodViaLinq(MethodInfo methodInfo, dynamic dto, dynamic entity,
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

        public static IStatusGeneric<object> RunMethodOrCtorViaLinq(MethodCtorMatch ctorOrMethod, dynamic dto, 
            ImmutableList<PropertyMatch> propertyMatches, DbContext context)
        {
            var result = new StatusGenericHandler<dynamic>();
            if (ctorOrMethod.Constructor != null)
            {
                var ctor = CallConstructor(ctorOrMethod.Constructor, dto.GetType(), propertyMatches);
                return propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext)
                    ? result.SetResult(ctor(dto, context))
                    : result.SetResult(ctor(dto));
            }

            //Otherwise its static method 
            var staticFunc = CallStaticFactory(ctorOrMethod.Method, dto.GetType(), propertyMatches);
            return propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext)
                ? staticFunc(dto, context)
                : staticFunc(dto);
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnVoidCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnVoid(MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnVoidCache.GetOrAdd(methodInfo.GenerateKey(), 
                type => PrivateCallMethodReturnVoid(methodInfo, tDto, tEntity, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnVoid(MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
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

        public static dynamic CallMethodReturnStatus(MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => PrivateCallMethodReturnStatus(methodInfo, tDto, tEntity, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnStatus(MethodInfo methodInfo, Type tDto, Type tEntity, IEnumerable<PropertyMatch> propertyMatches)
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

        public static dynamic CallStaticFactory(MethodInfo methodInfo, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(),
                type => PrivateCallStaticFactory(methodInfo, tDto, propertyMatches));
        }

        private static dynamic PrivateCallStaticFactory(MethodInfo methodInfo, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
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

        public static dynamic CallConstructor(ConstructorInfo ctor, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(ctor.GenerateKey(),
                type => PrivateCallConstructor(ctor, tDto, propertyMatches));
        }

        public static dynamic PrivateCallConstructor(ConstructorInfo ctor, Type tDto, IEnumerable<PropertyMatch> propertyMatches)
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