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
        //You should use Display, not DisplayName because Display accepts resource type and key, which can be used to provide translations using separate resource files (resx).
        //see https://pawanpalblog.wordpress.com/2016/03/16/using-displaydisplayname-attributes-in-mvc/

        public static string GetNameForProperty<T, TV>(this T source, Expression<Func<T, TV>> model) where T : class
        {
            var propAndAttr = GetPropAndAttr(model);
            return propAndAttr.Item2?.Description ?? propAndAttr.Item1.Name.SplitPascalCase();
        }

        public static string GetNameForClass<T>() where T : class
        {
            return GetNameForClass(typeof(T));
        }

        public static string GetNameForClass(this Type type)
        {
            var displayNameAttr = type.GetCustomAttribute<DescriptionAttribute>();
            return displayNameAttr?.Description ?? type.Name.SplitPascalCase();
        }

        //---------------------------------------------------------
        //private method

        private static Tuple<PropertyInfo, DescriptionAttribute> GetPropAndAttr<T, TV>(Expression<Func<T, TV>> model) where T : class
        {
            var memberEx = (MemberExpression)model.Body;
            if (memberEx == null)
                throw new ArgumentNullException(nameof(model), "You must supply a LINQ expression that is a property.");

            var propInfo = typeof(T).GetProperty(memberEx.Member.Name);
            if (propInfo == null)
                throw new ArgumentNullException(nameof(model), "The member you gave is not a property.");

            var displayAttr = propInfo.GetCustomAttribute<DescriptionAttribute>();
            return new Tuple<PropertyInfo, DescriptionAttribute>(propInfo, displayAttr);
        }
    }
}