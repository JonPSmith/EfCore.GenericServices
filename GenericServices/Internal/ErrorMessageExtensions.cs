// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]

namespace GenericServices.Internal
{
    internal static class ErrorMessageExtensions
    {
        public static string NicerToString(this MethodInfo member)
        {
            var name = (member.IsStatic ? "static method " : "method ") + member.Name;
            return name + "(" + string.Join(", ", member.GetParameters().Select(x => x.NicerToString())) + ")";
        }

        public static string NicerToString(this ConstructorInfo member)
        {
            return "ctor(" + string.Join(", ", member.GetParameters().Select(x => x.NicerToString())) + ")";
        }

        private static string NicerToString(this ParameterInfo parameter)
        {
            return NicerToString(parameter.ParameterType) + " " + parameter.Name;
        }

        private static string NicerToString(this Type type)
        {
            if (type.IsGenericType)
            {
                var genericParts = type.GetGenericArguments();
                return type.Name.Substring(0, type.Name.Length - 2) + "<" +
                       string.Join(",", genericParts.Select(x => x.NicerToString())) + ">";
            }

            return type.Name;
        }
    }
}