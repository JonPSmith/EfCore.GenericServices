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
using StatusGeneric;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal.LinqBuilders
{
    internal static class BuildCall
    {
        //BE WARNED: extension methods don't work with dynamics - got some strange error messages
        //Also the dto, entity and DbContext parameters cannot be object, but dynamic works

        public static IStatusGeneric RunMethodViaLinq(MethodInfo methodInfo, dynamic dto, dynamic entity,
            List<PropertyMatch> propertyMatches, dynamic context)
        {
            if (methodInfo.ReturnType == typeof(IStatusGeneric))
            {
                var func = CallMethodReturnStatus(methodInfo, dto.GetType(), entity.GetType(), propertyMatches);
                if (propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext))
                {
                    if (propertyMatches.Any())
                        return (IStatusGeneric) func(dto, entity, context);
                    return (IStatusGeneric)func(entity, context);
                }

                if (propertyMatches.Any())
                    return (IStatusGeneric)func(dto, entity);
                return (IStatusGeneric)func(entity);
            }

            //Otherwise its an action
            var action = CallMethodReturnVoid(methodInfo, dto.GetType(), entity.GetType(), propertyMatches);
            if (propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext))
            {
                if (propertyMatches.Any())
                    action(dto, entity, context);
                else
                    action(entity, context);
            }
            else
            {
                if (propertyMatches.Any())
                    action(dto, entity);
                else
                    action(entity);
            }
            return new StatusGenericHandler();
        }

        public static IStatusGeneric<object> RunMethodOrCtorViaLinq(MethodCtorMatch ctorOrMethod, dynamic dto, 
            List<PropertyMatch> propertyMatches, DbContext context)
        {
            var result = new StatusGenericHandler<object>();
            if (ctorOrMethod.Constructor != null)
            {
                var ctor = CallConstructor(ctorOrMethod.Constructor, dto.GetType(), propertyMatches);
                return propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext)
                    ? result.SetResult(ctor(dto, context))
                    : result.SetResult(ctor(dto));
            }

            //Otherwise its static method 
            var staticFunc = CallStaticCreator(ctorOrMethod.Method, dto.GetType(), propertyMatches);
            return propertyMatches.Any(x => x.MatchSource == MatchSources.DbContext)
                ? staticFunc(dto, context)
                : staticFunc(dto);
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnVoidCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnVoid(MethodInfo methodInfo, Type tDto, Type tEntity, List<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnVoidCache.GetOrAdd(methodInfo.GenerateKey(tDto), 
                type => PrivateCallMethodReturnVoid(methodInfo, tDto, tEntity, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnVoid(MethodInfo methodInfo, Type tDto, Type tEntity, List<PropertyMatch> propertyMatches)
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
                ? propertyMatches.Any()
                    ? Expression.Lambda(call, false, pIn, pCall)
                    : Expression.Lambda(call, false, pCall)
                : propertyMatches.Any()
                    ? Expression.Lambda(call, false, pIn, pCall, pContext)
                    : Expression.Lambda(call, false, pCall, pContext);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallMethodReturnStatusCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallMethodReturnStatus(MethodInfo methodInfo, Type tDto, Type tEntity, List<PropertyMatch> propertyMatches)
        {
            return CallMethodReturnStatusCache.GetOrAdd(methodInfo.GenerateKey(tDto),
                type => PrivateCallMethodReturnStatus(methodInfo, tDto, tEntity, propertyMatches));
        }

        private static dynamic PrivateCallMethodReturnStatus(MethodInfo methodInfo, Type tDto, Type tEntity, List<PropertyMatch> propertyMatches)
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
                ? propertyMatches.Any()
                    ? Expression.Lambda(call, false, pIn, pCall)
                    : Expression.Lambda(call, false, pCall)
                : propertyMatches.Any()
                    ? Expression.Lambda(call, false, pIn, pCall, pContext)
                    : Expression.Lambda(call, false, pCall, pContext);
            return built.Compile();
        }

        private static readonly ConcurrentDictionary<string, dynamic> CallStaticCreatorCache = new ConcurrentDictionary<string, dynamic>();

        public static dynamic CallStaticCreator(MethodInfo methodInfo, Type tDto, List<PropertyMatch> propertyMatches)
        {
            return CallStaticCreatorCache.GetOrAdd(methodInfo.GenerateKey(tDto),
                type => PrivateCallStaticCreator(methodInfo, tDto, propertyMatches));
        }

        private static dynamic PrivateCallStaticCreator(MethodInfo methodInfo, Type tDto, List<PropertyMatch> propertyMatches)
        {
            if (!propertyMatches.Any())
                throw new InvalidOperationException("I have not written this to handle static methods that take no parameters. I can do that but is it likely?");

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

        public static dynamic CallConstructor(ConstructorInfo ctor, Type tDto, List<PropertyMatch> propertyMatches)
        {
            return CallConstructorCache.GetOrAdd(ctor.GenerateKey(tDto),
                type => PrivateCallConstructor(ctor, tDto, propertyMatches));
        }

        public static dynamic PrivateCallConstructor(ConstructorInfo ctor, Type tDto, List<PropertyMatch> propertyMatches)
        {
            if (!propertyMatches.Any())
                throw new InvalidOperationException("I have not written this to handle constuctors that take no parameters. I can do that but is it likely?");

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

        private static string GenerateKey(this MethodInfo methodInfo, Type tDto)
        {
            return methodInfo.DeclaringType.FullName + tDto.FullName + (methodInfo.IsStatic ? "Static" : "") + methodInfo.ToString();
        }

        private static string GenerateKey(this ConstructorInfo ctorInfo, Type tDto)
        {
            return ctorInfo.DeclaringType.FullName + tDto.FullName + ctorInfo.ToString();
        }
    }
}