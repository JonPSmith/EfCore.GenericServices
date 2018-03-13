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
        public ConstructorInfo[] PublicCtors { get; private set; }
        public MethodInfo [] publicStaticFactoryMethods { get; private set; } = new MethodInfo[0];
        public MethodInfo[] publicSetterMethods { get; private set; }
        public PropertyInfo[] propertiesWithPublicSetter { get; private set; }

        public bool CanBeUpdatedViaProperties => propertiesWithPublicSetter.Any();
        public bool CanBeUpdatedViaMethods => publicSetterMethods.Any();

        public DecodedClass(Type classType)
        {
            PublicCtors = classType.GetConstructors();
            var allPublicProperties = classType.GetProperties();
            var methodNamesToIgnore = allPublicProperties.Where(pp => pp.GetMethod.IsPublic).Select(x => x.GetMethod.Name)
                .Union(allPublicProperties.Where(pp => pp.SetMethod.IsPublic).Select(x => x.SetMethod.Name)).ToArray();
            var methodsToInspect = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(pm => !methodNamesToIgnore.Contains(pm.Name)).ToArray();
            publicSetterMethods = methodsToInspect
                .Where(x => x.ReturnType == typeof(void) || 
                            x.ReturnType == typeof(IStatusGeneric)).ToArray();
            var staticMethods = methodsToInspect.Where(x => x.IsStatic).ToArray();
            if (staticMethods.Any())
            {
                publicStaticFactoryMethods = (from method in staticMethods
                    let genericArgs = (method.ReturnType.IsGenericType
                        ? method.ReturnType.GetGenericArguments()
                        : new Type[0])
                    where genericArgs.Length == 1 && genericArgs[0] == classType &&
                          method.ReturnType == typeof(IStatusGeneric<>)
                    select method).ToArray();
            }
            propertiesWithPublicSetter = allPublicProperties.Where(x => x.SetMethod.IsPublic).ToArray();
        }
    }
}