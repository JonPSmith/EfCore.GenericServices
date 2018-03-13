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
            return propAndAttr.Item2?.Name ?? propAndAttr.Item1.Name.SplitCamelCase();
        }

        public static string GetNameForClass<T>() where T : class
        {
            var displayNameAttr = typeof(T).GetCustomAttribute<DisplayNameAttribute>();
            return displayNameAttr?.DisplayName ?? typeof(T).Name.SplitCamelCase();
        }

        //public static string GetShortName<T, TV>(this T source, Expression<Func<T, TV>> model) where T : class
        //{
        //    var propAndAttr = GetPropAndAttr(model);
        //    return propAndAttr.Item2?.ShortName ?? propAndAttr.Item1.Name;
        //}

        //---------------------------------------------------------
        //private method

        private static Tuple<PropertyInfo,DisplayAttribute> GetPropAndAttr<T, TV>(Expression<Func<T, TV>> model) where T : class
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