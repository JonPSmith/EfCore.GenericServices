using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace GenericServices.Internal
{
    internal static class ExtractDisplayHelpers
    {
        //You should use DisplayName with ASP.NET Core
        //see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-2.1#dataannotations-localization

        public static string GetNameForProperty<T, TV>(this T source, Expression<Func<T, TV>> model) where T : class
        {
            var propAndAttr = GetPropAndAttr(model);
            return propAndAttr.Item2?.Name ?? propAndAttr.Item1.Name.SplitPascalCase();
        }

        public static string GetNameForClass<T>() where T : class
        {
            return GetNameForClass(typeof(T));
        }

        public static string GetNameForClass(this Type type)
        {
            var displayNameAttr = type.GetCustomAttribute<DisplayAttribute>();
            return displayNameAttr?.Name ?? type.Name.SplitPascalCase();
        }

        //---------------------------------------------------------
        //private method

        private static Tuple<PropertyInfo, DisplayAttribute> GetPropAndAttr<T, TV>(Expression<Func<T, TV>> model) where T : class
        {
            var memberEx = (MemberExpression)model.Body;
            if (memberEx == null)
                throw new ArgumentNullException(nameof(model), "You must supply a LINQ expression that is a property.");

            var propInfo = typeof(T).GetProperty(memberEx.Member.Name);
            if (propInfo == null)
                throw new ArgumentNullException(nameof(model), "The member you gave is not a property.");

            var displayAttr = propInfo.GetCustomAttribute<DisplayAttribute>();
            return new Tuple<PropertyInfo, DisplayAttribute>(propInfo, displayAttr);
        }
    }
}