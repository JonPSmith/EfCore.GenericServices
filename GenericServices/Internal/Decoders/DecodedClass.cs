// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using GenericLibsBase;

namespace GenericServices.Internal.Decoders
{
    internal class DecodedClass
    {
        public ConstructorInfo[] PublicCtors { get; }
        public MethodInfo [] PublicStaticFactoryMethods { get; } = new MethodInfo[0];
        public MethodInfo[] PublicSetterMethods { get; }
        public PropertyInfo[] PropertiesWithPublicSetter { get; }

        public bool CanBeUpdatedViaProperties => PropertiesWithPublicSetter.Any();
        public bool CanBeUpdatedViaMethods => PublicSetterMethods.Any();

        public DecodedClass(Type classType)
        {
            PublicCtors = classType.GetConstructors();
            var allPublicProperties = classType.GetProperties();
            var methodNamesToIgnore = allPublicProperties.Where(pp => pp.GetMethod.IsPublic).Select(x => x.GetMethod.Name)
                .Union(allPublicProperties.Where(pp => pp.SetMethod.IsPublic).Select(x => x.SetMethod.Name)).ToArray();
            var methodsToInspect = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(pm => !methodNamesToIgnore.Contains(pm.Name)).ToArray();
            PublicSetterMethods = methodsToInspect
                .Where(x => x.ReturnType == typeof(void) || 
                            x.ReturnType == typeof(IStatusGeneric)).ToArray();
            var staticMethods = classType.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            if (staticMethods.Any())
            {
                PublicStaticFactoryMethods = (from method in staticMethods
                    let genericArgs = (method.ReturnType.IsGenericType
                        ? method.ReturnType.GetGenericArguments()
                        : new Type[0])
                    where genericArgs.Length == 1 && genericArgs[0] == classType &&
                          method.ReturnType.GetInterface(nameof(IStatusGeneric)) != null
                    select method).ToArray();
            }
            PropertiesWithPublicSetter = allPublicProperties.Where(x => x.SetMethod.IsPublic).ToArray();
        }
    }
}